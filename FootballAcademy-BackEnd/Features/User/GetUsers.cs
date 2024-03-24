using AutoMapper;
using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Features.User;
using FootballAcademy_BackEnd.Shared;
using FootballAcademy_BackEnd.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Contracts.UrlNavigation;

namespace FootballAcademy_BackEnd.Features.User
{
    public static class GetUsers
    {
        public class GetUserParam : IFilterableSortableRoutePageParam, IRequest<Result<object>>
        {
            public string? search { get; set; }
            public string? sort { get; set; }
            public int? pageSize { get; set; }
            public int? pageNumber { get; set; }
        }

        internal sealed class Handler : IRequestHandler<GetUserParam, Result<object>>
        {
            FootballAcademyDBContext _dbContext;
            IMapper _mapper;
            public Handler(FootballAcademyDBContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }
            public async Task<Result<object>> Handle(GetUserParam request, CancellationToken cancellationToken)
            {
                var userQueryAble = _dbContext.User.Include(u => u.Staff)
                    .ThenInclude(s => s.StaffSpecialization)
                    .AsQueryable();

                var userQuery = new QueryBuilder<Entities.User>(userQueryAble)
                    .WithSearch(request?.search, "UserName")
                    .WithSort(request?.sort)
                    .Paginate(request?.pageNumber, request?.pageSize)
                    ;
                var response = await userQuery.BuildAsync();

                return Result.Success(response);
            }
        }
    }
}


public class GetUserEnpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/user/all",
        async (ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string? search, [FromQuery] string? sort) =>
        {
            var response = await sender.Send(new GetUsers.GetUserParam
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

            return Results.NotFound("Failed to Find User");
        })
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Paginator.PaginatedData<User>), StatusCodes.Status200OK))
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status401Unauthorized))
            .WithTags("User")
            ;
    }
}
