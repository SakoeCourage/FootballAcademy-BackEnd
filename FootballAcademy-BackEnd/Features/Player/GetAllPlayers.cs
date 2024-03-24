using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Features.Player;
using FootballAcademy_BackEnd.Shared;
using FootballAcademy_BackEnd.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static FootballAcademy_BackEnd.Contracts.UrlNavigation;

namespace FootballAcademy_BackEnd.Features.Player
{
    public static class GetAllPlayers
    {
        public class GetAllPlayerRequestData : IFilterableSortableRoutePageParam, IRequest<Result<object>>
        {
            public string? search { get; set; }
            public string? sort { get; set; }
            public int? pageSize { get; set; }
            public int? pageNumber { get; set; }
        }

        public class Handler : IRequestHandler<GetAllPlayerRequestData, Result<object>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dBContext)
            {
                _dbContext = dBContext;
            }
            public async Task<Result<object>> Handle(GetAllPlayerRequestData request, CancellationToken cancellationToken)
            {
                var playerQuery = _dbContext.Player.AsQueryable();

                var queryBuilder = new QueryBuilder<Entities.Player>(playerQuery)
                    .WithSearch(request?.search, "FirstName", "SurName", "Email")
                    .WithSort(request?.sort)
                    .Paginate(request?.pageNumber, request?.pageSize)
                    ;

                var response = await queryBuilder.BuildAsync();

                return Result.Success(response);
            }
        }

    }
}


public class getPlayersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/player/all",
        async (ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string? search, [FromQuery] string? sort) =>
        {
            var response = await sender.Send(new GetAllPlayers.GetAllPlayerRequestData
            {
                pageSize = pageSize,
                pageNumber = pageNumber,
                search = search,
                sort = sort
            });

            if (response.IsSuccess)
            {
                return Results.Ok(response?.Value);
            }

            return Results.NotFound("Failed to Find Player");
        })
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Paginator.PaginatedData<Player>), StatusCodes.Status200OK))
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status401Unauthorized))
            .WithTags("Player Registeration")
            ;
    }
}
