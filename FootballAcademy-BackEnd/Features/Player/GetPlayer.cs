using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Features.Player;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.Player
{
    public static class GetPlayer
    {
        public class GetPlayerRequestData : IRequest<Result<Entities.Player>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<GetPlayerRequestData, Result<Entities.Player>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dBContext)
            {
                _dbContext = dBContext;
            }

            public async Task<Result<Entities.Player>> Handle(GetPlayerRequestData request, CancellationToken cancellationToken)
            {
                var user = await _dbContext.Player.Include(x => x.TrainingGroup)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (user is null)
                {
                    return Result.Failure<Entities.Player>(Error.NotFound);
                }

                return Result.Success(user);
            }
        }
    }
}

public class GetPlayerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/player/{id}",
        async (Guid id, ISender sender) =>
        {
            var response = await sender.Send(new GetPlayer.GetPlayerRequestData
            {
                Id = id
            });

            if (response is null)
            {
                return Results.NotFound(response?.Error);
            }

            if (response.IsSuccess)
            {
                return Results.Ok(response?.Value);
            }

            return Results.NotFound("Failed to Find Player");
        })
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Player), StatusCodes.Status200OK))
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status401Unauthorized))
            .WithTags("Player Registeration")
            ;
    }
}
