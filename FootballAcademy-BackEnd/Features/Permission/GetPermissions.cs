using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Features.Permission;
using FootballAcademy_BackEnd.Shared;
using FootballAcademy_BackEnd.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static FootballAcademy_BackEnd.Contracts.UrlNavigation;

namespace FootballAcademy_BackEnd.Features.Permission
{
    public static class GetPermissions
    {
        public class getPermissionsRequest : IFilterableSortableRoutePageParam, IRequest<Result<object>>
        {
            public string? search { get; set; }
            public string? sort { get; set; }
            public int? pageSize { get; set; }
            public int? pageNumber { get; set; }
        }

        internal sealed class Handler : IRequestHandler<getPermissionsRequest, Result<object>>
        {

            private readonly FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Result<object>> Handle(getPermissionsRequest request, CancellationToken cancellationToken)
            {
                var permissionQuery = _dbContext.Permission.AsQueryable();

                var queryBuilder = new QueryBuilder<Entities.Permission>(permissionQuery)
                    .WithSearch(request?.search, "Name")
                    .WithSort(request?.sort)
                    .Paginate(request?.pageNumber, request?.pageSize)
                    ;

                var result = await queryBuilder.BuildAsync();

                return Result.Success(result);
            }
        }
    }
}

public class getPermissionsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapGet("api/permission/all",
        async (ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string? search, [FromQuery] string? sort) =>
        {

            var response = await sender.Send(new GetPermissions.getPermissionsRequest
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
        })
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Paginator.PaginatedData<Permission>), StatusCodes.Status200OK))
            .WithTags("Permission")
            ;
    }
}
