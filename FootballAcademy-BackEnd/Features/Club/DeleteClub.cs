using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.Club.DeleteClub;

namespace FootballAcademy_BackEnd.Features.Club
{
    public static class DeleteClub
    {
        public class DeleteClubRequest : IRequest<Result>
        {
            public Guid Id { get; set; }
        }


        public class Handler : IRequestHandler<DeleteClubRequest, Result>
        {
            private readonly FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dbContext)
            {

                _dbContext = dbContext;

            }

            public async Task<Result> Handle(DeleteClubRequest request, CancellationToken cancellationToken)
            {
                var affectedRows = await _dbContext
                    .Club
                    .Where(s => s.Id == request.Id)
                    .ExecuteDeleteAsync();

                if (affectedRows == 0) return Result.Failure(Error.NotFound);
                return Result.Success();
            }
        }
    }
}

public class MapDeleteClubEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/club/{Id}", async (ISender sender, Guid Id) =>
        {
            var response = await sender.Send(new DeleteClubRequest { Id = Id });

            if (response.IsFailure)
            {
                return Results.NotFound(response.Error);
            }

            return Results.NoContent();

        }).WithTags("Club")
              .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status204NoContent))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))

          ;
    }
}
