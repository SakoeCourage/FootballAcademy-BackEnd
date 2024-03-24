using AutoMapper;
using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Features.PlayerMentalAttribute;
using FootballAcademy_BackEnd.Shared;
using FootballAcademy_BackEnd.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static FootballAcademy_BackEnd.Contracts.UrlNavigation;

namespace FootballAcademy_BackEnd.Features.PlayerMentalAttribute
{
    public static class GetPlayersMentalAttributeList
    {
        public class GetPlayerMentalAttributeListRequest : IFilterableSortableRoutePageParam, IRequest<Result<object>>
        {
            public Guid Id { get; set; }
            public string? search { get; set; }
            public string? sort { get; set; }
            public int? pageSize { get; set; }
            public int? pageNumber { get; set; }
        }


        internal sealed class Handler : IRequestHandler<GetPlayerMentalAttributeListRequest, Result<object>>
        {
            FootballAcademyDBContext _dbContext;
            IMapper _mapper;
            public Handler(FootballAcademyDBContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<Result<object>> Handle(GetPlayerMentalAttributeListRequest request, CancellationToken cancellationToken)
            {
                var query = _dbContext.PlayerHasMentalAttribute.Where(x => x.PlayerId == request.Id).AsQueryable();

                var userQuery = new QueryBuilder<Entities.PlayerHasMentalAttribute>(query)
                    .WithSort(request?.sort)
                    .Paginate(request?.pageNumber, request?.pageSize)
                    ;
                var response = await userQuery.BuildAsync();

                return Result.Success(response);
            }
        }
    }

}

public class MappGetMentalAttributeEndpoint : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/player/{Id}/mental-attribute/all", async (Guid Id, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string? sort, ISender sender) =>
        {

            var result = await sender.Send(new GetPlayersMentalAttributeList.GetPlayerMentalAttributeListRequest
            {
                Id = Id,
                pageSize = pageSize,
                pageNumber = pageNumber,
                sort = sort

            });

            if (result.IsFailure)
            {
                return Results.UnprocessableEntity(result.Error);
            };

            return Results.Ok(result.Value);
        })
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status422UnprocessableEntity))
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Paginator.PaginatedData<PlayerHasMentalAttribute>), StatusCodes.Status200OK))
        .WithTags("Player Mental Attributes Assessment")
        ;
    }
}
