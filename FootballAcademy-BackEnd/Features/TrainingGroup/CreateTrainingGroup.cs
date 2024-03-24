using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using static FootballAcademy_BackEnd.Features.TrainingGroup.CreateTrainingGroup;

namespace FootballAcademy_BackEnd.Features.TrainingGroup
{
    public static class CreateTrainingGroup
    {
        public class TrainingGroupRequest : IRequest<Result<Guid>>
        {
            public string Name { get; set; } = String.Empty;

        }

        public class Validator : AbstractValidator<TrainingGroupRequest>
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
                            var exist = await dbContext.TrainingGroup.AnyAsync(s => s.Name == name, cancellationToken);
                            return !exist;
                        }
                    }).WithMessage("Training Group Already Exist")
                    ;


            }
        }

        public class Hanlder : IRequestHandler<TrainingGroupRequest, Result<Guid>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<TrainingGroupRequest> _validator;
            public Hanlder(FootballAcademyDBContext dbContext, IValidator<TrainingGroupRequest> validator)
            {

                _dbContext = dbContext;
                _validator = validator;

            }
            public async Task<Result<Guid>> Handle(TrainingGroupRequest request, CancellationToken cancellationToken)
            {
                var validationResponse = await _validator.ValidateAsync(request, cancellationToken);

                if (validationResponse.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResponse));
                }

                var newEntry = new Entities.TrainingGroup
                {
                    Name = request.Name
                };

                _dbContext.TrainingGroup.Add(newEntry);

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

public class CreateTrainingGroupEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/training-group", async (ISender sender, TrainingGroupRequest request) =>
        {
            var response = await sender.Send(request);
            if (response.IsFailure)
            {
                return Results.UnprocessableEntity(response.Error);
            }

            return Results.Ok(response.Value);

        }).WithTags("Training Group")
              .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status200OK))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))

          ;
    }
}