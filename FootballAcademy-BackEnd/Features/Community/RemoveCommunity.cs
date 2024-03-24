using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.Community.RemoveCommunity;

namespace FootballAcademy_BackEnd.Features.Community
{
    public static class RemoveCommunity
    {
        public class RemoveCommunityRequest : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<RemoveCommunityRequest, Result>
        {
            private readonly FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dbContext)
            {

                _dbContext = dbContext;

            }

            public async Task<Result> Handle(RemoveCommunityRequest request, CancellationToken cancellationToken)
            {
                var affectedRows = await _dbContext
                    .Community
                    .Where(s => s.Id == request.Id)
                    .ExecuteDeleteAsync();

                if (affectedRows == 0) return Result.Failure(Error.NotFound);
                return Result.Success();
            }
        }
    }
}

public class MapDeleteCommunityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/community/{Id}", async (ISender sender, Guid Id) =>
        {
            var response = await sender.Send(new RemoveCommunityRequest { Id = Id });

            if (response.IsFailure)
            {
                return Results.NotFound(response.Error);
            }

            return Results.NoContent();

        }).WithTags("Community")
              .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status204NoContent))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))
          ;
    }
}