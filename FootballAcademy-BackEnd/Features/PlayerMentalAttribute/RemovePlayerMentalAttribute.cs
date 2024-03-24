using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.PlayerMentalAttribute;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.PlayerMentalAttribute
{
    public static class RemovePlayerMentalAttribute
    {
        public class RemovePlayerAttributeRequestData : IRequest<Result>
        {
            public Guid Id { get; set; }
            public Guid PlayerId { get; set; }

        }

        public class Handler : IRequestHandler<RemovePlayerAttributeRequestData, Result>
        {
            private readonly FootballAcademyDBContext _dbContext;

            public Handler(FootballAcademyDBContext dBContext)
            {
                _dbContext = dBContext;
            }
            public async Task<Result> Handle(RemovePlayerAttributeRequestData request, CancellationToken cancellationToken)
            {
                var affectedRoles = await _dbContext
                  .PlayerHasMentalAttribute
                  .Where(x => x.Id == request.Id && x.PlayerId == request.PlayerId)
                  .ExecuteDeleteAsync();

                if (affectedRoles == 0) return Result.Failure(Error.NotFound);

                return Result.Success(request.Id);


            }
        }

    }
}

public class RemoveAttributeEndpoint : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/player/{PlayerId}/mental-attribute/{MentalAttributeId}", async (Guid PlayerId, Guid MentalAttributeId, ISender sender) =>
        {
            var result = await sender.Send(
                new RemovePlayerMentalAttribute.RemovePlayerAttributeRequestData
                {
                    Id = MentalAttributeId,
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
        .WithTags("Player Mental Attributes Assessment")
        ;
        ;
    }
}
