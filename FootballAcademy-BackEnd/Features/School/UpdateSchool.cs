using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.School.UpdateSchool;

namespace FootballAcademy_BackEnd.Features.School
{
    public static class UpdateSchool
    {

        public class UpdateSchoolRequest
        {
            public string Name { get; set; } = String.Empty;
            public string? PhoneNumber { get; set; } = String.Empty;
            public string? Location { get; set; } = String.Empty;
        }

        public class UpdateSchoolRequestData : IRequest<Result<Guid>>
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = String.Empty;
            public string? PhoneNumber { get; set; } = String.Empty;
            public string? Location { get; set; } = String.Empty;
        }

        public class Validator : AbstractValidator<UpdateSchoolRequestData>
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
                            var exist = await dbContext.School.AnyAsync(s => s.Name == name && s.Id != model.Id, cancellationToken);
                            return !exist;
                        }
                    }).WithMessage("School Name Already Exist")
                    ;
                RuleFor(c => c.PhoneNumber).NotEmpty();
                RuleFor(c => c.Location).NotEmpty();
            }
        }

        public class Hanlder : IRequestHandler<UpdateSchoolRequestData, Result<Guid>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<UpdateSchoolRequestData> _validator;
            public Hanlder(FootballAcademyDBContext dbContext, IValidator<UpdateSchoolRequestData> validator)
            {

                _dbContext = dbContext;
                _validator = validator;

            }
            public async Task<Result<Guid>> Handle(UpdateSchoolRequestData request, CancellationToken cancellationToken)
            {
                var validationResponse = await _validator.ValidateAsync(request, cancellationToken);

                if (validationResponse.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResponse));
                }

                var affectedRows = await _dbContext.School.Where(x => x.Id == request.Id)
                    .ExecuteUpdateAsync(setters =>
                          setters.SetProperty(p => p.Location, request.Location)
                          .SetProperty(p => p.Name, request.Name)
                          .SetProperty(p => p.PhoneNumber, request.PhoneNumber)
                          .SetProperty(p => p.UpdatedAt, DateTime.UtcNow)

                    );
                if (affectedRows == 0) return Result.Failure<Guid>(Error.NotFound);

                return Result.Success<Guid>(request.Id);
            }
        }

    }
}

public class MapUpdateSchoolEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/school/{Id}", async (ISender sender, Guid Id, UpdateSchoolRequest request) =>
        {
            var response = await sender.Send(
                new UpdateSchoolRequestData
                {
                    Id = Id,
                    Location = request.Location,
                    Name = request.Name,
                    PhoneNumber = request.PhoneNumber
                }
                );

            if (response.IsFailure)
            {
                return Results.UnprocessableEntity(response.Error);
            }

            return Results.Ok(response.Value);

        }).WithTags("School")
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Guid), StatusCodes.Status200OK))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest));
    }
}


