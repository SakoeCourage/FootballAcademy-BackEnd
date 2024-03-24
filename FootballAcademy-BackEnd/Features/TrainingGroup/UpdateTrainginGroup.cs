using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.TrainingGroup.UpdateTrainginGroup;

namespace FootballAcademy_BackEnd.Features.TrainingGroup
{
    public static class UpdateTrainginGroup
    {
        public class UpdateTraingGroupRequest
        {
            public string Name { get; set; } = String.Empty;

        }

        public class UpdateTraingGroupRequestData : IRequest<Result<Guid>>
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = String.Empty;

        }
        public class Validator : AbstractValidator<UpdateTraingGroupRequestData>
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
                            var exist = await dbContext.TrainingGroup.AnyAsync(s => s.Name == name && s.Id != model.Id, cancellationToken);
                            return !exist;
                        }
                    }).WithMessage("Training Group Already Exist")
                    ;
            }
        }

        public class Hanlder : IRequestHandler<UpdateTraingGroupRequestData, Result<Guid>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<UpdateTraingGroupRequestData> _validator;
            public Hanlder(FootballAcademyDBContext dbContext, IValidator<UpdateTraingGroupRequestData> validator)
            {

                _dbContext = dbContext;
                _validator = validator;

            }
            public async Task<Result<Guid>> Handle(UpdateTraingGroupRequestData request, CancellationToken cancellationToken)
            {
                var validationResponse = await _validator.ValidateAsync(request, cancellationToken);

                if (validationResponse.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResponse));
                }

                var affectedRows = await _dbContext.TrainingGroup.Where(x => x.Id == request.Id)
                    .ExecuteUpdateAsync(setters =>
                          setters
                          .SetProperty(p => p.Name, request.Name)
                          .SetProperty(p => p.UpdatedAt, DateTime.UtcNow)

                    );
                if (affectedRows == 0) return Result.Failure<Guid>(Error.NotFound);

                return Result.Success<Guid>(request.Id);
            }
        }
    }
}

public class MapUpdateTrainginGroupEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/traing-group/{Id}", async (ISender sender, Guid Id, UpdateTraingGroupRequest request) =>
        {
            var response = await sender.Send(
                new UpdateTraingGroupRequestData
                {
                    Id = Id,
                    Name = request.Name
                }
                );

            if (response.IsFailure)
            {
                return Results.UnprocessableEntity(response.Error);
            }

            return Results.Ok(response.Value);

        }).WithTags("Training Group")
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Guid), StatusCodes.Status200OK))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest));
    }
}
