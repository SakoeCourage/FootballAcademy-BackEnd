using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Shared;
using FootballAcademy_BackEnd.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static FootballAcademy_BackEnd.Contracts.UrlNavigation;
using static FootballAcademy_BackEnd.Features.TrainingGroup.GetTrainingGroupList;

namespace FootballAcademy_BackEnd.Features.TrainingGroup
{
    public static class GetTrainingGroupList
    {
        public class GetTrainingGroupListRequest : IFilterableSortableRoutePageParam, IRequest<Result<object>>
        {
            public string? search { get; set; }
            public string? sort { get; set; }
            public int? pageSize { get; set; }
            public int? pageNumber { get; set; }
        }

        public class Handler : IRequestHandler<GetTrainingGroupListRequest, Result<object>>
        {
            private readonly FootballAcademyDBContext _dBContext;
            public Handler(FootballAcademyDBContext dbContext)
            {
                _dBContext = dbContext;
            }
            public async Task<Result<object>> Handle(GetTrainingGroupListRequest request, CancellationToken cancellationToken)
            {
                var query = _dBContext.TrainingGroup.AsQueryable();

                var queryBuilder = new QueryBuilder<Entities.TrainingGroup>(query)
                        .WithSearch(request?.search, "Name")
                        .WithSort(request?.sort)
                        .Paginate(request?.pageNumber, request?.pageSize);

                var response = await queryBuilder.BuildAsync();

                return Result.Success(response);
            }
        }
    }
}


public class GetTrainingGroupListEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/training-group/all", async (ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string? search, [FromQuery] string? sort) =>
        {

            var response = await sender.Send(new GetTrainingGroupListRequest
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
          .WithMetadata(new ProducesResponseTypeAttribute(typeof(Paginator.PaginatedData<TrainingGroup>), StatusCodes.Status200OK))
          .WithTags("Training Group");
    }
}