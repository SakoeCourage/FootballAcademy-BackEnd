using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Shared;
using FootballAcademy_BackEnd.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Contracts.UrlNavigation;
using static FootballAcademy_BackEnd.Features.Coach.GetCoachList;

namespace FootballAcademy_BackEnd.Features.Coach
{
    public static class GetCoachList
    {
        public class GetCoachListRequest : IFilterableSortableRoutePageParam, IRequest<Result<object>>
        {
            public string? search { get; set; }
            public string? sort { get; set; }
            public int? pageSize { get; set; }
            public int? pageNumber { get; set; }
        }

        public class Handler : IRequestHandler<GetCoachListRequest, Result<object>>
        {
            private readonly FootballAcademyDBContext _dBContext;
            public Handler(FootballAcademyDBContext dbContext)
            {
                _dBContext = dbContext;
            }
            public async Task<Result<object>> Handle(GetCoachListRequest request, CancellationToken cancellationToken)
            {
                var query = _dBContext.Staff.AsQueryable();

                query = query.Include(x => x.StaffSpecialization).Where(x => x.StaffSpecialization.Name == "Coach");

                var queryBuilder = new QueryBuilder<Entities.Staff>(query)
                        .WithSearch(request?.search, "FirstName", "LastName")
                        .WithSort(request?.sort)
                        .Paginate(request?.pageNumber, request?.pageSize);

                var response = await queryBuilder.BuildAsync();

                return Result.Success(response);
            }
        }
    }
}

public class MapGetCoachesListEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/coaches/all", async (ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string? search, [FromQuery] string? sort) =>
        {

            var response = await sender.Send(new GetCoachListRequest
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
          .WithMetadata(new ProducesResponseTypeAttribute(typeof(Paginator.PaginatedData<Staff>), StatusCodes.Status200OK))
          .WithTags("Coach");
    }
}
