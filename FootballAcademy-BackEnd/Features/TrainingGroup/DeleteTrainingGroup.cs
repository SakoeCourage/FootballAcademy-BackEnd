using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.TrainingGroup.DeleteTrainingGroup;

namespace FootballAcademy_BackEnd.Features.TrainingGroup
{
    public static class DeleteTrainingGroup
    {
        public class DeleteTrainingGroupRequest : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        internal sealed class Handler : IRequestHandler<DeleteTrainingGroupRequest, Result>
        {
            private readonly FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<Result> Handle(DeleteTrainingGroupRequest request, CancellationToken cancellationToken)
            {
                var affectedColumns = await _dbContext
                    .TrainingGroup
                    .Where(c => c.Id == request.Id)
                    .ExecuteDeleteAsync();

                if (affectedColumns is 0) return Result.Failure(Error.NotFound);

                return Result.Success();
            }
        }
    }
}

public class MapDeleteTrainingGroupEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("ap/training-group/{Id}", async (ISender sender, Guid Id) =>
        {
            var request = new DeleteTrainingGroupRequest { Id = Id };

            var result = await sender.Send(request);

            if (result.IsFailure is true)
            {
                return Results.NotFound(result.Error);
            }

            return Results.NoContent();
        }).WithTags("Training Group")
              .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status204NoContent))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest));
        ;
    }
}
