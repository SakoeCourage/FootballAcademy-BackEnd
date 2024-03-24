using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Shared;
using FootballAcademy_BackEnd.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static FootballAcademy_BackEnd.Contracts.UrlNavigation;
using static FootballAcademy_BackEnd.Features.PlayerTechnicalSkill.GetPlayerTechnicalSkillList;

namespace FootballAcademy_BackEnd.Features.PlayerTechnicalSkill
{
    public static class GetPlayerTechnicalSkillList
    {
        public class GetPlayerTechnicallSkillListRequest : IFilterableSortableRoutePageParam, IRequest<Result<object>>
        {
            public Guid Id { get; set; }
            public string? search { get; set; }
            public string? sort { get; set; }
            public int? pageSize { get; set; }
            public int? pageNumber { get; set; }
        }


        internal sealed class Handler : IRequestHandler<GetPlayerTechnicallSkillListRequest, Result<object>>
        {
            FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Result<object>> Handle(GetPlayerTechnicallSkillListRequest request, CancellationToken cancellationToken)
            {
                var query = _dbContext.PlayerHasTechnicalSkills.Where(x => x.PlayerId == request.Id).AsQueryable();

                var userQuery = new QueryBuilder<Entities.PlayerHasTechnicalSkills>(query)
                    .WithSort(request?.sort)
                    .Paginate(request?.pageNumber, request?.pageSize)
                    ;
                var response = await userQuery.BuildAsync();

                return Result.Success(response);
            }
        }
    }
}

public class GetPlayerTechnicalSkillListEndpoint : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/player/{Id}/technical-skill/all", async (Guid Id, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string? sort, ISender sender) =>
        {

            var result = await sender.Send(new GetPlayerTechnicallSkillListRequest
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
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Paginator.PaginatedData<PlayerHasTechnicalSkills>), StatusCodes.Status200OK))
        .WithTags("Player Technical Skill Assessment")
        ;
        ;
    }
}
