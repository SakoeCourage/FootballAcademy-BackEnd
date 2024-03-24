using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.PlayerTechnicalSkill;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.PlayerTechnicalSkill
{
    public static class RemovePlayerTechnicalSkill
    {
        public class RemovePlayerTechnicalSkillRequestData : IRequest<Result>
        {
            public Guid Id { get; set; }
            public Guid PlayerId { get; set; }

        }

        public class Handler : IRequestHandler<RemovePlayerTechnicalSkillRequestData, Result>
        {
            private readonly FootballAcademyDBContext _dbContext;

            public Handler(FootballAcademyDBContext dBContext)
            {
                _dbContext = dBContext;

            }
            public async Task<Result> Handle(RemovePlayerTechnicalSkillRequestData request, CancellationToken cancellationToken)
            {
                var affectedRoles = await _dbContext
                  .PlayerHasTechnicalSkills
                  .Where(x => x.Id == request.Id && x.PlayerId == request.PlayerId)
                  .ExecuteDeleteAsync(cancellationToken);

                if (affectedRoles == 0) return Result.Failure(Error.NotFound);

                return Result.Success(request.Id);


            }
        }
    }
}

public class RemovePlayerTechnicalSkillEndpoint : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/player/{PlayerId}/technical-skill/{TechnicalSkilId}", async (Guid PlayerId, Guid TechnicalSkilId, ISender sender) =>
        {
            var result = await sender.Send(
                new RemovePlayerTechnicalSkill.RemovePlayerTechnicalSkillRequestData
                {
                    Id = TechnicalSkilId,
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
        .WithTags("Player Technical Skill Assessment")
        ;
        ;
    }
}

