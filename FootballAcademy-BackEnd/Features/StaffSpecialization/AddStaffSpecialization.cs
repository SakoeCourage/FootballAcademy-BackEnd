using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Features.StaffSpecialization;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;

namespace FootballAcademy_BackEnd.Features.StaffSpecialization
{
    public static class AddStaffSpecialization
    {
        public class NewStaffSpecializationDTO : IRequest<Result>
        {
            public string Name { get; set; }
        }

        public class Validator : AbstractValidator<NewStaffSpecializationDTO>
        {
            private readonly IServiceScopeFactory _serviceScopeFactory;
            public Validator(IServiceScopeFactory serviceScopeFactory)
            {

                _serviceScopeFactory = serviceScopeFactory;

                RuleFor(c => c.Name)
                    .NotEmpty()
                    .MustAsync(async (name, cancellationToken) =>
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<FootballAcademyDBContext>();
                            bool exist = await dbContext.StaffSpecialization.AnyAsync(sp => sp.Name.ToLower() == name.Trim().ToLower());
                            return !exist;
                        }
                    })
                    .WithMessage("Name Already Exist");
            }
        }

        internal sealed class Hander : IRequestHandler<NewStaffSpecializationDTO, Result>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<NewStaffSpecializationDTO> _validator;
            public Hander(FootballAcademyDBContext dbContext, IValidator<NewStaffSpecializationDTO> validator)
            {

                _dbContext = dbContext;
                _validator = validator;

            }
            public async Task<Result> Handle(NewStaffSpecializationDTO request, CancellationToken cancellationToken)
            {
                if (request == null) return Result.InvalidRequest();

                var validationResponse = await _validator.ValidateAsync(request);

                if (!validationResponse.IsValid)
                {
                    return Result.Failure(new Error(StatusCodes.Status422UnprocessableEntity.ToString(), validationResponse));
                }

                var newStaffSpecialization = new Entities.StaffSpecialization
                {
                    Name = request.Name,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _dbContext.AddAsync(newStaffSpecialization);
                await _dbContext.SaveChangesAsync();

                return Result.Success(newStaffSpecialization);
            }
        }
    }
}

public class AddSpecializationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/staff-specialization",

            async (AddStaffSpecialization.NewStaffSpecializationDTO request, ISender sender) =>
            {
                var response = await sender.Send(request);

                if (response.IsFailure) {
                    return Results.UnprocessableEntity(response.Error);
                }

                return Results.NoContent();
            }
            ).WithTags("Staff Specialization")
            ;
    }
}
