using AutoMapper;
using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.User;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FootballAcademy_BackEnd.Features.User
{
    public static class UpdateUser
    {
        public class UpdateUserDTO : IRequest<Result<Guid>>
        {
            public Guid Id { get; set; }
            public string UserName { get; set; } = String.Empty;
            [EmailAddress]
            public string Email { get; set; } = String.Empty;
            public string FirstName { get; set; } = String.Empty;
            public string LastName { get; set; }
            public string? OtherNames { get; set; }
            public Guid StaffSpecializationId { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Phone { get; set; }
            public Guid RoleId { get; set; }
            public string? Qualification { get; set; } = String.Empty;
            public string? PassportPicture { get; set; }

        }

        public class Validator : AbstractValidator<UpdateUserDTO>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Validator(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
                RuleFor(c => c.Email).EmailAddress().NotEmpty()
                 .MustAsync(async (model, email, cancellation) =>
                 {
                     using (var scope = _scopeFactory.CreateScope())
                     {
                         var dbContext = scope.ServiceProvider.GetRequiredService<FootballAcademyDBContext>();
                         bool exists = await dbContext.User
                             .AnyAsync(e => e.Email.ToLower() == email.ToLower() && e.Id != model.Id)
                             ;
                         return !exists;
                     }
                 })
                 .WithMessage("Email Address Already Exist");

                RuleFor(c => c.UserName).NotEmpty().MinimumLength(3);
                RuleFor(c => c.Phone).NotEmpty();
                RuleFor(c => c.FirstName).NotEmpty();
                RuleFor(c => c.LastName).NotEmpty();
                RuleFor(c => c.StaffSpecializationId).NotEmpty()
                    .WithMessage("Staffs Specialization Cannot Be Empty")
                    .MustAsync(async (id, cancellation) =>
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<FootballAcademyDBContext>();
                            bool exists = await dbContext.StaffSpecialization
                                .AnyAsync(e => e.Id == id);
                            return exists;
                        }
                    })
                    .WithMessage("Staff Specialization Not Found");
            }
        }

        internal sealed class Handler : IRequestHandler<UpdateUserDTO, Result<Guid>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<UpdateUserDTO> _validator;
            private readonly IMapper _mapper;
            public Handler(FootballAcademyDBContext dbContext, IValidator<UpdateUserDTO> validator, IMapper mapper)
            {
                _dbContext = dbContext;
                _validator = validator;
                _mapper = mapper;

            }
            public async Task<Result<Guid>> Handle(UpdateUserDTO request, CancellationToken cancellationToken)
            {
                if (request == null)
                {
                    return Result.Failure<Guid>(new Error("Invalid Request", "Invlaid Body Type"));
                }
                var validationResult = await _validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {

                    return Result.Failure<Guid>(new Error(StatusCodes.Status422UnprocessableEntity.ToString(), validationResult));
                }

                var user = await _dbContext.User.FindAsync(request.Id);
                var userStaffData = await _dbContext.Staff.FirstOrDefaultAsync(u => u.UserId == user.Id)
                    ;
                if (user is null || userStaffData is null)
                {
                    return (Result<Guid>)Result.InvalidRequest();
                }

                user.Email = request.Email;
                user.RoleId = request.RoleId;
                user.UserName = request.UserName;
                user.UpdatedAt = DateTime.UtcNow;

                userStaffData.FirstName = request.FirstName;
                userStaffData.LastName = request.LastName;
                userStaffData.OtherNames = request.OtherNames;
                userStaffData.Qualification = request.Qualification;
                userStaffData.PassportPicture = request.PassportPicture;
                userStaffData.Phone = request.Phone;
                userStaffData.DateOfBirth = request.DateOfBirth;
                userStaffData.StaffSpecializationId = request.StaffSpecializationId;
                userStaffData.UpdatedAt = DateTime.UtcNow;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    return Result.Success<Guid>(user.Id);
                }
                catch (DbUpdateException ex)
                {
                    return Result.Failure<Guid>(new Error(StatusCodes.Status400BadRequest.ToString(), "Error updating user: " + ex.Message));
                }


            }
        }

    }
}
public class CreateUpdateUserEndpoint : ICarterModule
{
    private readonly IMapper _mapper;

    public CreateUpdateUserEndpoint(IMapper mapper)
    {
        _mapper = mapper;

    }
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/user/{id}", async (Guid id, FootballAcademy_BackEnd.Contracts.UpdateUserRequest request, ISender sender) =>
        {
            var req = _mapper.Map<UpdateUser.UpdateUserDTO>(request);
            req.Id = id;
            var result = await sender.Send(req);
            if (result.IsFailure)
            {
                return Results.UnprocessableEntity(result.Error);
            };

            return Results.Ok(result.Value);
        })
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status422UnprocessableEntity))
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Guid), StatusCodes.Status200OK))
        .WithTags("User")
        ;
        ;
    }
}