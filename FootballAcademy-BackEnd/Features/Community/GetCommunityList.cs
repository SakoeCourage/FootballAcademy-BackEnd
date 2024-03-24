using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Shared;
using FootballAcademy_BackEnd.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static FootballAcademy_BackEnd.Contracts.UrlNavigation;
using static FootballAcademy_BackEnd.Features.Community.GetCommunityList;

namespace FootballAcademy_BackEnd.Features.Community
{
    public static class GetCommunityList
    {
        public class GetCommunityRequest : IFilterableSortableRoutePageParam, IRequest<Result<object>>
        {
            public string? search { get; set; }
            public string? sort { get; set; }
            public int? pageSize { get; set; }
            public int? pageNumber { get; set; }
        }

        public class Handler : IRequestHandler<GetCommunityRequest, Result<object>>
        {
            private readonly FootballAcademyDBContext _dBContext;
            public Handler(FootballAcademyDBContext dbContext)
            {
                _dBContext = dbContext;
            }
            public async Task<Result<object>> Handle(GetCommunityRequest request, CancellationToken cancellationToken)
            {
                var schoolQuery = _dBContext.Community.AsQueryable();

                var schoolQueryBuilder = new QueryBuilder<Entities.Community>(schoolQuery)
                        .WithSearch(request?.search, "CommunityName")
                        .WithSort(request?.sort)
                        .Paginate(request?.pageNumber, request?.pageSize);

                var response = await schoolQueryBuilder.BuildAsync();

                return Result.Success(response);
            }
        }

    }
}

public class GetCommunityListEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/community/all", async (ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string? search, [FromQuery] string? sort) =>
        {

            var response = await sender.Send(new GetCommunityRequest
            {
                pageSize = pageSize,
                pageNumber = pageNumber,
                search = search,
                sort = sort
            });

            if (response is null)
            {
                return Results.BadRequest("Empty Result");
            }

            if (response.IsSuccess)
            {
                return Results.Ok(response.Value);
            }


            return Results.BadRequest("Empty Result");
        }).WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))
          .WithMetadata(new ProducesResponseTypeAttribute(typeof(Paginator.PaginatedData<Community>), StatusCodes.Status200OK))
          .WithTags("Community");
    }

}
