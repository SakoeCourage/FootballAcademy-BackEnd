using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Features.Permission;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FootballAcademy_BackEnd.Features.Permission
{
    public static class GetPermission
    {
        public class GetPermissionRequest : IRequest<Result<Entities.Permission?>>
        {
            public Guid Id { get; set; }
        }

        internal sealed class Hanlder : IRequestHandler<GetPermissionRequest, Result<Entities.Permission?>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            public Hanlder(FootballAcademyDBContext dBContext)
            {
                _dbContext = dBContext;
            }
            public async Task<Result<Entities.Permission?>> Handle(GetPermissionRequest request, CancellationToken cancellationToken)
            {
                var permission = await _dbContext.Permission.FindAsync(request.Id);

                if (permission is null) return Result.Failure<Entities.Permission?>(Error.NotFound);

                return Result.Success<Entities.Permission?>(permission);

            }
        }
    }
}

public class CreateGetPermissionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/permission/{Id}", async (ISender sender, Guid Id) =>
        {
            var result = await sender.Send(new GetPermission.GetPermissionRequest
            {
                Id = Id
            });
            if (result is null)
            {
                return Results.BadRequest(result?.Error);
            }
            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return Results.BadRequest(result?.Error);

        }).WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Permission), StatusCodes.Status200OK))
            .WithTags("Permission")
            ;
    }
}
