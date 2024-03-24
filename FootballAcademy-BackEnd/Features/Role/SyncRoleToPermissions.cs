using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Features.Role;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.Role
{
    public static class SyncRoleToPermissions
    {

        public class RequestData : IRequest<Result>
        {
            public List<string> PermissionNames { get; set; }
        }

        public class DataToProcess : IRequest<Result>
        {

            public Guid RoleId { get; set; }
            public List<string> PermissionNames { get; set; }

        }
        public class Validator : AbstractValidator<DataToProcess>
        {
            public Validator()
            {
                RuleFor(x => x.RoleId)
                .NotEmpty();

                RuleFor(c => c.PermissionNames)
                .NotNull().WithMessage("Permissions list cannot be null.")
                .Must(permissions => permissions != null && permissions.Count > 0)
                .WithMessage("At least one permission ID must be provided.");
            }

        }


        internal sealed class Handler : IRequestHandler<DataToProcess, Result>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<DataToProcess> _validator;
            public Handler(FootballAcademyDBContext dbContext, IValidator<DataToProcess> validator)
            {
                _dbContext = dbContext;
                _validator = validator;

            }
            public async Task UpdateRolePermissions(Guid roleId, List<string> permissionNames)
            {
                var existingPermissions = await _dbContext.RoleHasPermissions
                                                        .Include(rp => rp.Permission)
                                                        .Where(rp => rp.RoleId == roleId)
                                                        .ToListAsync();

                var existingPermissionNames = existingPermissions.Select(rp => rp.Permission.Name).ToList();

                var permissionIds = await _dbContext.Permission
                                                .Where(p => permissionNames.Contains(p.Name))
                                                .Select(p => p.Id)
                                                .ToListAsync();

                var newPermissionIds = permissionIds.Except(existingPermissions.Select(rp => rp.PermissionId));

                foreach (var newPermissionId in newPermissionIds)
                {
                    _dbContext.RoleHasPermissions.Add(new RoleHasPermissions
                    {
                        RoleId = roleId,
                        PermissionId = newPermissionId
                    });
                }

                var removedPermissions = existingPermissions.Where(rp => !permissionNames.Contains(rp.Permission.Name)).ToList();

                _dbContext.RoleHasPermissions.RemoveRange(removedPermissions);

                await _dbContext.SaveChangesAsync();
            }



            public async Task<Result> Handle(DataToProcess request, CancellationToken cancellation)
            {

                var validationResponse = _validator.Validate(request);

                if (!validationResponse.IsValid)
                {
                    return Result.Failure(new Error(StatusCodes.Status422UnprocessableEntity.ToString(), validationResponse.Errors));
                }

                try
                {
                    await UpdateRolePermissions(request.RoleId, request.PermissionNames);
                }
                catch (Exception ex) 
                {
                    return Result.Failure(new Error(StatusCodes.Status422UnprocessableEntity.ToString(), ex.Message));
                }

                return Result.Success();
            }

        }
    }
}

public class SyncRolePermissionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/role/sync-permissions/{roleId}", async (Guid roleId, SyncRoleToPermissions.RequestData request, ISender sender) =>
        {

            var response = await sender.Send(
                new SyncRoleToPermissions.DataToProcess {
                    RoleId = roleId,
                    PermissionNames = request.PermissionNames
                }
                );

            if (response.IsFailure)
            {
                return Results.UnprocessableEntity(response.Error);
            }

            return Results.NoContent();
        }).WithTags("Role")
            ;
    }
}



