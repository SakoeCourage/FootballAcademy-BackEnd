using MediatR;
using FootballAcademy_BackEnd.Shared;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.User.CreateUser;
using Carter;
using FootballAcademy_BackEnd.Features.Permission;

namespace FootballAcademy_BackEnd.Features.Permission
{
    public class CreatePermission
    {
        public class CreatePermissionDTO : IRequest<Result<Guid>>
        {
            public String Name { get; set; }
        }

        public class Validator : AbstractValidator<CreatePermissionDTO>
        {
            protected readonly IServiceScopeFactory _serviceScopeFactory;
            public Validator(IServiceScopeFactory scopeServiceFactory)
            {
                _serviceScopeFactory = scopeServiceFactory;

                RuleFor(x => x.Name)
                    .NotEmpty()
                    .MustAsync(async (name, cancellationToken) =>
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<FootballAcademyDBContext>();
                            bool exist = await dbContext
                            .Permission
                            .AnyAsync(e => e.Name.ToLower() == name.Trim().ToLower());
                            return !exist;
                        }

                    })
                    .WithMessage("Permission Already Exist")
                    ;
            }
        }

        internal sealed class HandleRequest : IRequestHandler<CreatePermissionDTO, Result<Guid>>
        {
            protected readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<CreatePermissionDTO> _validator;
            public HandleRequest(FootballAcademyDBContext dbContext, IValidator<CreatePermissionDTO> validator)
            {
                _dbContext = dbContext;
                _validator = validator;
            }

            public async Task<Result<Guid>> Handle(CreatePermissionDTO request, CancellationToken cancellationToken)
            {

                if (request == null) return Result.Failure<Guid>(new Error(code: "Invalid Request", message: "Invalid Request body"));

                var validationResult = await _validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    return Result.Failure<Guid>(new Error(StatusCodes.Status422UnprocessableEntity.ToString(), validationResult));
                }


                var newPermission = new Entities.Permission
                {
                    Name = request.Name,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _dbContext.Add(newPermission);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return newPermission.Id;
            }
        }
    }
}

public class CreatePermissionEndpint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/permission",
            async (CreatePermission.CreatePermissionDTO request, ISender sender) =>
            {
              
                var response = await sender.Send(request);
                if (response.IsFailure)
                {
                    return Results.UnprocessableEntity(response.Error);
                }
                return Results.Ok(response.Value);
            }
        ).WithTags("Permission")

            ;
    }
}



