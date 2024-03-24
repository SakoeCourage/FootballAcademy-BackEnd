using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Features.StaffSpecialization;
using FootballAcademy_BackEnd.Shared;
using FootballAcademy_BackEnd.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static FootballAcademy_BackEnd.Contracts.UrlNavigation;

namespace FootballAcademy_BackEnd.Features.StaffSpecialization
{
    public static class GetStaffSpecializationList
    {
        public class GetStaffSpecializationsRequest : IFilterableSortableRoutePageParam, IRequest<Result<object>>
        {
            public string? search { get; set; }
            public string? sort { get; set; }
            public int? pageSize { get; set; }
            public int? pageNumber { get; set; }
        }

        internal sealed class Handler : IRequestHandler<GetStaffSpecializationsRequest, Result<object>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<Result<object>> Handle(GetStaffSpecializationsRequest request, CancellationToken cancellationToken)
            {
                var query = _dbContext.StaffSpecialization.AsQueryable();

                var spquery = new
                    QueryBuilder<Entities.StaffSpecialization>(query)
                    .WithSearch(request?.search, "Name")
                    .WithSort(request?.sort)
                    .Paginate(request?.pageNumber, request?.pageSize);

                var response = await spquery.BuildAsync();

                return Result.Success(response);
            }
        }
    }
}

public class GetStaffSpecializationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/staff-specialization/all",
        async (ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string? search, [FromQuery] string? sort) =>
        {
            var response = await sender.Send(new GetStaffSpecializationList.GetStaffSpecializationsRequest
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

            return Results.NotFound("Failed to Find Specialization");
        })
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Paginator.PaginatedData<StaffSpecialization>), StatusCodes.Status200OK))
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status401Unauthorized))
            .WithTags("Staff Specialization")
            ;
    }
}
