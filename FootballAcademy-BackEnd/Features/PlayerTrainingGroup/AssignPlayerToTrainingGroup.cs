using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.PlayerTrainingGroup.AssignPlayerToTrainingGroup;

namespace FootballAcademy_BackEnd.Features.PlayerTrainingGroup
{
    public static class AssignPlayerToTrainingGroup
    {
        public class AssignmentRequest : IRequest<Result>
        {
            public Guid PlayerId { get; set; }
            public Guid TrainingGroupId { get; set; }
        }

        internal sealed class Handler : IRequestHandler<AssignmentRequest, Result>
        {
            private readonly FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dBContext)
            {
                _dbContext = dBContext;
            }

            public async Task<Result> Handle(AssignmentRequest request, CancellationToken cancellationToken)
            {
                var PlayerIdExist = await _dbContext.Player.FirstOrDefaultAsync(x => x.Id == request.PlayerId);
                var TrainingGroupExist = await _dbContext.TrainingGroup.FirstOrDefaultAsync(x => x.Id == request.TrainingGroupId);

                if (PlayerIdExist is null | TrainingGroupExist is null)
                {
                    return Result.Failure(Error.NotFound);
                }

                var existingEntry = await _dbContext.PlayerHasTrainingGroup.AsNoTracking().FirstOrDefaultAsync(x => x.PlayerId == request.PlayerId);

                if (existingEntry is not null)
                {
                    //Delete Operation 
                    var deletedRow = await _dbContext
                    .PlayerHasTrainingGroup
                    .AsNoTracking()
                    .Where(x => x.PlayerId == request.PlayerId)
                    .ExecuteDeleteAsync();

                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                var newEntry = await _dbContext.PlayerHasTrainingGroup.AddAsync(new Entities.PlayerHasTrainingGroup
                {
                    PlayerId = request.PlayerId,
                    TrainingGroupId = request.TrainingGroupId
                });

                await _dbContext.SaveChangesAsync();

                return Result.Success();
            }
        }
    }
}

public class AssignPlayerToTrainingGroupEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/player/{PlayerId}/add-or-update-training-group/{TrainingGroupId}", async (Guid PlayerId, Guid TrainingGroupId, ISender sender) =>
        {
            var result = await sender.Send(new AssignmentRequest
            {
                PlayerId = PlayerId,
                TrainingGroupId = TrainingGroupId
            });

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            };

            if (result.IsSuccess)
            {
                return Results.NoContent();
            }
            return Results.BadRequest();
        })
       .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status422UnprocessableEntity))
       .WithMetadata(new ProducesResponseTypeAttribute(typeof(Guid), StatusCodes.Status200OK))
       .WithTags("Players Training Group");
    }
}
