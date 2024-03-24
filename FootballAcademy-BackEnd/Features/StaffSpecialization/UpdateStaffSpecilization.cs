using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.StaffSpecialization;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.StaffSpecialization
{
    public static class UpdateStaffSpecilization
    {

        public class UpdateRequestBody
        {
            public string Name { get; set; }
        }
        public class UpdateRequest : IRequest<Result<string>>
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Validator : AbstractValidator<UpdateRequest>
        {
            private readonly IServiceScopeFactory _scopeFactory;
            public Validator(IServiceScopeFactory scopeFactory)
            {

                _scopeFactory = scopeFactory;

                RuleFor(c => c.Name)
                    .NotEmpty()
                    .MustAsync(async (model, name, cancellation) =>
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetService<FootballAcademyDBContext>();
                            var exist = await dbContext.StaffSpecialization.AnyAsync(r => r.Name.ToLower() == name.Trim().ToLower() && r.Id != model.Id);
                            return !exist;
                        }
                    }
                    )
                    .WithMessage("Staff Specilization Already Exist");
                ;
            }
        }

        internal sealed class Hander : IRequestHandler<UpdateRequest, Result<string>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<UpdateRequest> _validator;
            public Hander(FootballAcademyDBContext dbContext, IValidator<UpdateRequest> validator)
            {

                _dbContext = dbContext;
                _validator = validator;

            }
            public async Task<Result<string>> Handle(UpdateRequest request, CancellationToken cancellationToken)
            {
                if (request == null) return Result.InvalidRequest<string>();

                var validationResponse = await _validator.ValidateAsync(request);

                if (!validationResponse.IsValid)
                {
                    return Result.Failure<string>(new Error(StatusCodes.Status422UnprocessableEntity.ToString(), validationResponse));
                }

                var staffSpecilization = await _dbContext.StaffSpecialization.FirstOrDefaultAsync(sp => sp.Id == request.Id);

                if (staffSpecilization is null)
                {
                    return Result.Failure<string>(Error.NotFound);
                }

                staffSpecilization.Name = request.Name;
                staffSpecilization.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();
                return Result.Success("Staff Specialization Updated Successfully");
            }
        }
    }
}

public class UpdateStaffSpecializationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/staff-specialization/{Id}",
        async (ISender sender, Guid Id, [FromBody] UpdateStaffSpecilization.UpdateRequestBody request) =>
        {
            var response = await sender.Send(new UpdateStaffSpecilization.UpdateRequest
            {
                Id = Id,
                Name = request.Name,
            });


            if (response.IsSuccess)
            {
                return Results.Ok(response?.Value);
            }

            return Results.BadRequest(response?.Error);
        })
            .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status200OK))
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status401Unauthorized))
            .WithTags("Staff Specialization")
            ;
    }
}

