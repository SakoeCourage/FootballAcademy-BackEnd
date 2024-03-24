using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.PlayerTacticallSkill;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.PlayerTacticallSkill
{
    public static class RemovePlayerTacticalSkill
    {
        public class RemovePlayerTacticalSkillRequestData : IRequest<Result>
        {
            public Guid Id { get; set; }
            public Guid PlayerId { get; set; }

        }

        public class Handler : IRequestHandler<RemovePlayerTacticalSkillRequestData, Result>
        {
            private readonly FootballAcademyDBContext _dbContext;

            public Handler(FootballAcademyDBContext dBContext)
            {
                _dbContext = dBContext;

            }
            public async Task<Result> Handle(RemovePlayerTacticalSkillRequestData request, CancellationToken cancellationToken)
            {
                var affectedRoles = await _dbContext
                  .PlayerHasTacticalSkills
                  .Where(x => x.Id == request.Id && x.PlayerId == request.PlayerId)
                  .ExecuteDeleteAsync(cancellationToken);

                if (affectedRoles == 0) return Result.Failure(Error.NotFound);

                return Result.Success(request.Id);


            }
        }
    }
}

public class RemovePlayerTacticalSkillEndpoint : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/player/{PlayerId}/tactical-skill/{TacticalSkillId}", async (Guid PlayerId, Guid TacticalSkillId, ISender sender) =>
        {
            var result = await sender.Send(
                new RemovePlayerTacticalSkill.RemovePlayerTacticalSkillRequestData
                {
                    Id = TacticalSkillId,
                    PlayerId = PlayerId
                }
                );

            if (result.IsFailure)
            {
                return Results.UnprocessableEntity(result.Error);
            };

            return Results.NoContent();
        })
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status422UnprocessableEntity))
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Guid), StatusCodes.Status200OK))
        .WithTags("Player Tactical Skill Assessment")
        ;
        ;
    }
}

