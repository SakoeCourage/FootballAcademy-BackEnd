using AutoMapper;
using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Entities;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static FootballAcademy_BackEnd.Features.User.CreateUser;

namespace FootballAcademy_BackEnd.Features.User
{
    public static class CreateUser
    {
        public class CreateUserDTO : IRequest<Result<Guid>>
        {
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

        public class Validator : AbstractValidator<CreateUserDTO>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Validator(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
                RuleFor(c => c.Email).EmailAddress().NotEmpty()
                         .MustAsync(async (email, cancellation) =>
                         {
                             using (var scope = _scopeFactory.CreateScope())
                             {
                                 var dbContext = scope.ServiceProvider.GetRequiredService<FootballAcademyDBContext>();
                                 bool exists = await dbContext.User
                                     .AnyAsync(e => e.Email.ToLower() == email.ToLower());
                                 return !exists;
                             }
                         })
                         .WithMessage("Email Address Already Exist")
                ;

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

        internal sealed class Handler : IRequestHandler<CreateUserDTO, Result<Guid>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<CreateUserDTO> _validator;
            private readonly IMapper _mapper;
            public Handler(FootballAcademyDBContext dbContext, IValidator<CreateUserDTO> validator, IMapper mapper)
            {
                _dbContext = dbContext;
                _validator = validator;
                _mapper = mapper;

            }
            public async Task<Result<Guid>> Handle(CreateUserDTO request, CancellationToken cancellationToken)
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

                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {

                        var defaultPassword = $"{DateTime.UtcNow.Year}{request.FirstName}";
                        Console.WriteLine(defaultPassword);
                        //Creating A New User
                        var user = new Entities.User
                        {
                            UserName = request.UserName,
                            Email = request.Email,
                            RoleId = request.RoleId,
                            Password = BCrypt.Net.BCrypt.HashPassword(defaultPassword),
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                        };

                        _dbContext.Add(user);
                        await _dbContext.SaveChangesAsync(cancellationToken);

                        //New User ID
                        var newUserId = user.Id;

                        // Adding User Staff Data
                        var userStaffData = _mapper.Map<Staff>(request);
                        userStaffData.UserId = newUserId;
                        userStaffData.CreatedAt = DateTime.UtcNow;
                        userStaffData.UpdatedAt = DateTime.UtcNow;

                        await _dbContext.AddAsync(userStaffData);

                        //Adding To User Role Pivot Table
                        var userHasRole = new UserHasRole
                        {
                            UserId = newUserId,
                            RoleId = request.RoleId
                        };
                        await _dbContext.UserHasRole.AddAsync(userHasRole);

                        await _dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return newUserId;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result.Failure<Guid>(new Error(StatusCodes.Status400BadRequest.ToString(), ex.Message));
                    }
                }
                return Result.Failure<Guid>(new Error(StatusCodes.Status400BadRequest.ToString(), "Something Wrong"));


            }
        }

    }

}

public class CreateUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/user", async (CreateUserDTO request, ISender sender) =>
        {
            var result = await sender.Send(request);
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