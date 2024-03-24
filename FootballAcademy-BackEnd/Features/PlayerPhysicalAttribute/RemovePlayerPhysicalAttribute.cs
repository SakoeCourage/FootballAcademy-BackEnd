using AutoMapper;
using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.PlayerPhysicalAttribute;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.PlayerPhysicalAttribute.UpdatePlayerPhysicalAttribute;

namespace FootballAcademy_BackEnd.Features.PlayerPhysicalAttribute
{
    public static class RemovePlayerPhysicalAttribute
    {
        public class RemovePlayerAttributeRequestData : IRequest<Result>
        {
            public Guid Id { get; set; }
            public Guid PlayerId { get; set; }

        }

        public class Handler : IRequestHandler<RemovePlayerAttributeRequestData, Result>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<UpdatePlayerPhysicalAttributeRequestData> _validator;
            private readonly IMapper _mapper;

            public Handler(FootballAcademyDBContext dBContext, IValidator<UpdatePlayerPhysicalAttributeRequestData> validator, IMapper mapper)
            {
                _dbContext = dBContext;
                _validator = validator;
                _mapper = mapper;
            }
            public async Task<Result> Handle(RemovePlayerAttributeRequestData request, CancellationToken cancellationToken)
            {
                var affectedRoles = await _dbContext
                  .PlayerHasPhysicalAttribute
                  .Where(x => x.Id == request.Id && x.PlayerId == request.PlayerId)
                  .ExecuteDeleteAsync();

                if (affectedRoles == 0) return Result.Failure(Error.NotFound);

                return Result.Success(request.Id);


            }
        }

    }
}

public class RemovePlayerEndpoint : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/player/{PlayerId}/physical-attribute/{PysicalAttributeId}", async (Guid PlayerId, Guid PysicalAttributeId, ISender sender) =>
        {
            var result = await sender.Send(
                new RemovePlayerPhysicalAttribute.RemovePlayerAttributeRequestData
                {
                    Id = PysicalAttributeId,
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
        .WithTags("Player Physical Attributes Assessment")
        ;
        ;
    }
}
