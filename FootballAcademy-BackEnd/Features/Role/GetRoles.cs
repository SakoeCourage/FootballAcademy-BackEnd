using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Features.Role;
using FootballAcademy_BackEnd.Shared;
using FootballAcademy_BackEnd.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Contracts.UrlNavigation;

namespace FootballAcademy_BackEnd.Features.Role
{
    public static class GetRoles
    {
        public class GetRoleRequest : IFilterableSortableRoutePageParam, IRequest<Result<object>>
        {
            public string? search { get; set; }
            public string? sort { get; set; }
            public int? pageSize { get; set; }
            public int? pageNumber { get; set; }
        }

        internal sealed class Handler : IRequestHandler<GetRoleRequest, Result<object>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<Result<object>> Handle(GetRoleRequest request, CancellationToken cancellationToken)
            {
                var initialQuery = _dbContext.Role.Include(r => r.Permissions).AsQueryable();

                var queryBuilder = new QueryBuilder<Entities.Role>(initialQuery);
                Console.WriteLine("page size", request?.pageSize);
                queryBuilder
                    .WithSearch(request?.search, "Name")
                    .WithSort(request?.sort)
                    .Paginate(request?.pageNumber, request?.pageSize);
                ;

                var result = await queryBuilder.BuildAsync();

                return Result.Success(result);
            }
        }
    }
}

public class GetRolesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapGet("api/role/all",
        async (ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string? search, [FromQuery] string? sort) =>
        {

            var response = await sender.Send(new GetRoles.GetRoleRequest
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
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Paginator.PaginatedData<Role>), StatusCodes.Status200OK))
            .WithTags("Role")
            ;
    }
}
