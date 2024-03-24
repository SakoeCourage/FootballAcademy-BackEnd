using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.Role;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.Role
{
    public static class CreateRole
    {

        public class CreateRoleDTO : IRequest<Result<Guid>> { 
            public string Name { get; set; }
        }

        public class Validator : AbstractValidator<CreateRoleDTO> {
            private readonly IServiceScopeFactory _scopeFactory;
            public Validator(IServiceScopeFactory scopeFactory)
            {

                _scopeFactory = scopeFactory;

                RuleFor(c => c.Name)
                    .NotEmpty()
                    .MustAsync(async (name, cancellation) =>
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetService<FootballAcademyDBContext>();
                            var exist = await dbContext.Role.AnyAsync(r => r.Name.ToLower() == name.Trim().ToLower());
                            return !exist;
                        }
                    }
                    )
                    .WithMessage("Role Name Already Exist");
                    ;
            }
        }

        internal sealed class RequestHanlder : IRequestHandler<CreateRoleDTO, Result<Guid>> {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<CreateRoleDTO> _validator;
            public RequestHanlder(FootballAcademyDBContext dbContext, IValidator<CreateRoleDTO> validator )
            {
                _dbContext = dbContext;
                _validator = validator;
            }
            public async Task<Result<Guid>> Handle(CreateRoleDTO request,CancellationToken cancellationtoken) {

                if (request == null) return Result.InvalidRequest<Guid>();  
                
                var validationResponse = await _validator.ValidateAsync(request);

                if (!validationResponse.IsValid) {
                    return Result.Failure<Guid>(new Error(StatusCodes.Status422UnprocessableEntity.ToString(), validationResponse));
                }

                var newRole = new Entities.Role
                {
                    Name = request.Name,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _dbContext.Add(newRole);
                await _dbContext.SaveChangesAsync();
                return newRole.Id;

            }
        }
    }
}


public class NewRoleEndPoint: ICarterModule {
    public  void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPost("api/role", async (CreateRole.CreateRoleDTO request,ISender sender) =>
        {
            var response = await sender.Send(request);
            if (response.IsFailure) {
                return Results.UnprocessableEntity(response.Error);
            }

            return Results.Ok(response.Value);
        }).WithTags("Role")
            ;
    }
}
