using AutoMapper;
using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Features.PlayerPhysicalAttribute;
using FootballAcademy_BackEnd.Shared;
using FootballAcademy_BackEnd.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static FootballAcademy_BackEnd.Contracts.UrlNavigation;

namespace FootballAcademy_BackEnd.Features.PlayerPhysicalAttribute
{
    public static class GetPlayersPhysicalAttributeList
    {
        public class GetPlayerPhysicalAttributeListRequest : IFilterableSortableRoutePageParam, IRequest<Result<object>>
        {
            public Guid Id { get; set; }
            public string? search { get; set; }
            public string? sort { get; set; }
            public int? pageSize { get; set; }
            public int? pageNumber { get; set; }
        }


        internal sealed class Handler : IRequestHandler<GetPlayerPhysicalAttributeListRequest, Result<object>>
        {
            FootballAcademyDBContext _dbContext;
            IMapper _mapper;
            public Handler(FootballAcademyDBContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<Result<object>> Handle(GetPlayerPhysicalAttributeListRequest request, CancellationToken cancellationToken)
            {
                var query = _dbContext.PlayerHasPhysicalAttribute.Where(x => x.PlayerId == request.Id).AsQueryable();

                var userQuery = new QueryBuilder<Entities.PlayerHasPhysicalAttribute>(query)
                    .WithSort(request?.sort)
                    .Paginate(request?.pageNumber, request?.pageSize)
                    ;
                var response = await userQuery.BuildAsync();

                return Result.Success(response);
            }
        }
    }

}

public class MappGetAttributeEndpoint : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/player/{Id}/physical-attribute/all", async (Guid Id, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string? sort, ISender sender) =>
        {

            var result = await sender.Send(new GetPlayersPhysicalAttributeList.
                GetPlayerPhysicalAttributeListRequest
            {
                Id = Id,
                pageNumber = pageNumber,
                pageSize = pageSize,
                sort = sort

            });

            if (result.IsFailure)
            {
                return Results.UnprocessableEntity(result.Error);
            };

            return Results.Ok(result.Value);
        })
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status422UnprocessableEntity))
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Paginator.PaginatedData<PlayerHasPhysicalAttribute>), StatusCodes.Status200OK))
        .WithTags("Player Physical Attributes Assessment")
        ;
        ;
    }
}
