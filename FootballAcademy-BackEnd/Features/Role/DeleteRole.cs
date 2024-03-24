using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.Role;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.Role
{
    public static class DeleteRole
    {

        public class DeleteRoleRequest : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        internal sealed class Hanlder : IRequestHandler<DeleteRoleRequest, Result>
        {
            FootballAcademyDBContext _dbContext;
            public Hanlder(FootballAcademyDBContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<Result> Handle(DeleteRoleRequest request, CancellationToken cancellationToken)
            {
                var role = await _dbContext.Role.FindAsync(request.Id, cancellationToken);

                if (role is null)
                {
                    return Result.Failure(Error.NotFound);
                }

                _dbContext.Role.Remove(role);
                try
                {
                    await _dbContext.SaveChangesAsync();
                    return Result.Success("Role Deleted Succesfully");
                }
                catch (DbUpdateException ex)
                {
                    return Result.Failure(Error.BadRequest(ex.Message));
                }
            }
        }
    }
}

public class MapDeleteUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/role/{Id}", async (Guid Id, ISender sender) =>
        {
            var response = await sender.Send(new DeleteRole.DeleteRoleRequest
            {
                Id = Id
            });

            if (response.IsFailure)
            {
                return Results.BadRequest(response.Error);
            }
            if (response.IsSuccess)
            {
                return Results.Ok("Role Deleted Successfully");
            }
            return Results.BadRequest(response.Error);

        }).WithTags("Role")
              .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status200OK))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))

        ;
    }
}