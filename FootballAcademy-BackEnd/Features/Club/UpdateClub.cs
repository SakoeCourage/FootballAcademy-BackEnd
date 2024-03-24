using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.Club.UpdateClub;

namespace FootballAcademy_BackEnd.Features.Club
{
    public static class UpdateClub
    {
        public class UpdateClubRequest
        {
            public string Name { get; set; } = String.Empty;
            public string? Contact { get; set; }
            public string? Location { get; set; }

        }
        public class UpdateClubRequestData : IRequest<Result<Guid>>
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = String.Empty;
            public string? Contact { get; set; }
            public string? Location { get; set; }

        }

        public class Validator : AbstractValidator<UpdateClubRequestData>
        {
            private readonly IServiceScopeFactory _scopeFactory;
            public Validator(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
                RuleFor(c => c.Name).NotEmpty()
                    .MustAsync(async (model, name, cancellationToken) =>
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetService<FootballAcademyDBContext>();
                            var exist = await dbContext.Club.AnyAsync(s => s.Name == name && s.Id != model.Id, cancellationToken);
                            return !exist;
                        }
                    }).WithMessage("Club Name Already Exist")
                    ;
                RuleFor(c => c.Contact).NotEmpty();

            }
        }

        public class Handler : IRequestHandler<UpdateClubRequestData, Result<Guid>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<UpdateClubRequestData> _validator;
            public Handler(FootballAcademyDBContext dbContext, IValidator<UpdateClubRequestData> validator)
            {

                _dbContext = dbContext;
                _validator = validator;

            }
            public async Task<Result<Guid>> Handle(UpdateClubRequestData request, CancellationToken cancellationToken)
            {
                var validationResponse = await _validator.ValidateAsync(request, cancellationToken);

                if (validationResponse.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResponse));
                }

                var affectedRows = await _dbContext.Club.Where(x => x.Id == request.Id)
                    .ExecuteUpdateAsync(setters =>
                          setters.SetProperty(p => p.Location, request.Location)
                          .SetProperty(p => p.Name, request.Name)
                          .SetProperty(p => p.Contact, request.Contact)
                          .SetProperty(p => p.UpdatedAt, DateTime.UtcNow)

                    );
                if (affectedRows == 0) return Result.Failure<Guid>(Error.NotFound);

                return Result.Success<Guid>(request.Id);
            }
        }
    }
}

public class MapUpdateClubEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/club/{Id}", async (ISender sender, Guid Id, UpdateClubRequest request) =>
        {
            var response = await sender.Send(
                new UpdateClubRequestData
                {
                    Id = Id,
                    Location = request.Location,
                    Name = request.Name,
                    Contact = request.Contact
                }
                );

            if (response.IsFailure)
            {
                return Results.UnprocessableEntity(response.Error);
            }

            return Results.Ok(response.Value);

        }).WithTags("Club")
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Guid), StatusCodes.Status200OK))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest));
    }
}
