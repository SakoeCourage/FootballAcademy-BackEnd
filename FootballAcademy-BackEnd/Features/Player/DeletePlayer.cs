using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.Player;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.Player
{
    public static class DeletePlayer
    {
        public class DeletePlayerRequest : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        internal sealed class Hander : IRequestHandler<DeletePlayerRequest, Result>
        {
            protected readonly FootballAcademyDBContext _dbContext;
            public Hander(FootballAcademyDBContext dBContext)
            {
                _dbContext = dBContext;
            }
            public async Task<Result> Handle(DeletePlayerRequest request, CancellationToken cancellationToken)
            {
                var deletedRow = await _dbContext.Player.Where(ent => ent.Id == request.Id)
                    .ExecuteDeleteAsync(cancellationToken);
                ;
                if (deletedRow == 0) return Result.Failure(Error.NotFound);

                return Result.Success();
            }
        }
    }
}

public class DeletePlayerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/player/{Id}", async (ISender sender, Guid Id) =>
        {
            var result = await sender.Send(new DeletePlayer.DeletePlayerRequest { Id = Id });

            if (result.IsFailure)
            {
                return Results.BadRequest(result?.Error);
            }
            if (result.IsSuccess)
            {
                return Results.NoContent();
            }

            return Results.BadRequest();
        })
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status422UnprocessableEntity))
        .WithTags("Player Registeration")
          ;
    }
}
