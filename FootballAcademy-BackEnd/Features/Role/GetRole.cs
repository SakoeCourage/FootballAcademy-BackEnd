using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Features.Role;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.Role
{
    public static class GetRole
    {

        public class GetReleRequest : IRequest<Result<Entities.Role>>
        {
            public Guid Id { get; set; }
        }


        internal sealed class Handler : IRequestHandler<GetReleRequest, Result<Entities.Role>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<Result<Entities.Role>> Handle(GetReleRequest request, CancellationToken cancellationToken)
            {
                var role = await _dbContext.Role.Include(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == request.Id);

                if (role is null)
                {
                    return Result.Failure<Entities.Role>(Error.NotFound);
                }
                return Result.Success(role);
            }
        }
    }
}


public class GetRoleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapGet("api/role/{Id}",
        async (ISender sender, Guid id) =>
        {

            var response = await sender.Send(new GetRole.GetReleRequest
            {
                Id = id
            });

            if (response.IsFailure)
            {
                return Results.BadRequest(response?.Error);
            }

            if (response.IsSuccess)
            {
                return Results.Ok(response.Value);
            }

            return Results.BadRequest(response?.Error);
        })
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Role), StatusCodes.Status200OK))
            .WithTags("Role")
            ;
    }
}