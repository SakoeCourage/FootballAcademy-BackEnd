using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using static FootballAcademy_BackEnd.Features.Club.AddClub;

namespace FootballAcademy_BackEnd.Features.Club
{
    public static class AddClub
    {
        public class AddClubRequest : IRequest<Result<Guid>>
        {
            public string Name { get; set; } = String.Empty;
            public string? Contact { get; set; }
            public string? Location { get; set; }

        }

        public class Validator : AbstractValidator<AddClubRequest>
        {
            private readonly IServiceScopeFactory _scopeFactory;
            public Validator(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
                RuleFor(c => c.Name).NotEmpty()
                    .MustAsync(async (name, cancellationToken) =>
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetService<FootballAcademyDBContext>();
                            var exist = await dbContext.Club.AnyAsync(s => s.Name == name, cancellationToken);
                            return !exist;
                        }
                    }).WithMessage("Club Name Already Exist")
                    ;
                RuleFor(c => c.Contact).NotEmpty();


            }
        }

        public class Hanlder : IRequestHandler<AddClubRequest, Result<Guid>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<AddClubRequest> _validator;
            public Hanlder(FootballAcademyDBContext dbContext, IValidator<AddClubRequest> validator)
            {

                _dbContext = dbContext;
                _validator = validator;

            }
            public async Task<Result<Guid>> Handle(AddClubRequest request, CancellationToken cancellationToken)
            {
                var validationResponse = await _validator.ValidateAsync(request, cancellationToken);

                if (validationResponse.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResponse));
                }

                var newEntry = new Entities.Club
                {
                    Name = request.Name,
                    Contact = request.Contact,
                    Location = request.Location

                };

                _dbContext.Club.Add(newEntry);

                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbException ex)
                {
                    return Result.Failure<Guid>(Error.BadRequest(ex.Message));
                }
                return Result.Success<Guid>(newEntry.Id);
            }
        }

    }
}

public class MappAddClubRequest : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/club", async (ISender sender, AddClubRequest request) =>
        {
            var response = await sender.Send(request);
            if (response.IsFailure)
            {
                return Results.UnprocessableEntity(response.Error);
            }

            return Results.Ok(response.Value);

        }).WithTags("Club")
              .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status200OK))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))

          ;
    }
}
