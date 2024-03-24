using AutoMapper;
using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Shared;
using FootballAcademy_BackEnd.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static FootballAcademy_BackEnd.Contracts.UrlNavigation;
using static FootballAcademy_BackEnd.Features.PlayerTacticallSkill.GetPlayerTacticalSkillList;

namespace FootballAcademy_BackEnd.Features.PlayerTacticallSkill
{
    public static class GetPlayerTacticalSkillList
    {
        public class GetPlayerTacticallSkillListRequest : IFilterableSortableRoutePageParam, IRequest<Result<object>>
        {
            public Guid Id { get; set; }
            public string? search { get; set; }
            public string? sort { get; set; }
            public int? pageSize { get; set; }
            public int? pageNumber { get; set; }
        }


        internal sealed class Handler : IRequestHandler<GetPlayerTacticallSkillListRequest, Result<object>>
        {
            FootballAcademyDBContext _dbContext;
            IMapper _mapper;
            public Handler(FootballAcademyDBContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<Result<object>> Handle(GetPlayerTacticallSkillListRequest request, CancellationToken cancellationToken)
            {
                var query = _dbContext.PlayerHasTacticalSkills.Where(x => x.PlayerId == request.Id).AsQueryable();

                var userQuery = new QueryBuilder<Entities.PlayerHasTacticalSkills>(query)
                    .WithSort(request?.sort)
                    .Paginate(request?.pageNumber, request?.pageSize)
                    ;
                var response = await userQuery.BuildAsync();

                return Result.Success(response);
            }
        }
    }
}

public class GetPlayerTacticalSkillListEndpoint : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/player/{Id}/tactical-skill/all", async (Guid Id, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string? sort, ISender sender) =>
        {

            var result = await sender.Send(new
                GetPlayerTacticallSkillListRequest
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
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Paginator.PaginatedData<PlayerHasTacticalSkills>), StatusCodes.Status200OK))
        .WithTags("Player Tactical Skill Assessment")
        ;
        ;
    }
}
