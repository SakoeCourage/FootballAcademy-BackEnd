using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using static FootballAcademy_BackEnd.Features.School.CreateSchool;

namespace FootballAcademy_BackEnd.Features.School
{
    public static class CreateSchool
    {
        public class CreateSchoolRequest : IRequest<Result<Guid>>
        {
            public string Name { get; set; } = String.Empty;
            public string? PhoneNumber { get; set; } = String.Empty;
            public string? Location { get; set; } = String.Empty;
        }

        public class Validator : AbstractValidator<CreateSchoolRequest>
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
                            var exist = await dbContext.School.AnyAsync(s => s.Name == name, cancellationToken);
                            return !exist;
                        }
                    }).WithMessage("School Name Already Exist")
                    ;
                RuleFor(c => c.PhoneNumber).NotEmpty();
                RuleFor(c => c.Location).NotEmpty();
            }
        }

        public class Hanlder : IRequestHandler<CreateSchoolRequest, Result<Guid>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<CreateSchoolRequest> _validator;
            public Hanlder(FootballAcademyDBContext dbContext, IValidator<CreateSchoolRequest> validator)
            {

                _dbContext = dbContext;
                _validator = validator;

            }
            public async Task<Result<Guid>> Handle(CreateSchoolRequest request, CancellationToken cancellationToken)
            {
                var validationResponse = await _validator.ValidateAsync(request, cancellationToken);

                if (validationResponse.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResponse));
                }

                var newSchool = new Entities.School
                {
                    Name = request.Name,
                    Location = request.Location,
                    PhoneNumber = request.PhoneNumber
                };

                _dbContext.School.Add(newSchool);

                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbException ex)
                {
                    return Result.Failure<Guid>(Error.BadRequest(ex.Message));
                }
                return Result.Success<Guid>(newSchool.Id);
            }
        }

    }
}

public class MapCreateSchoolEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/school", async (ISender sender, CreateSchoolRequest request) =>
        {
            var response = await sender.Send(request);
            if (response.IsFailure)
            {
                return Results.UnprocessableEntity(response.Error);
            }

            return Results.Ok(response.Value);

        }).WithTags("School")
              .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status200OK))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))

          ;
    }
}
