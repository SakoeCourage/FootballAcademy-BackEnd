using AutoMapper;
using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Contracts;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.Auth;
using FootballAcademy_BackEnd.Providers;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FootballAcademy_BackEnd.Features.Auth
{
    public static class Login
    {
        public record StaffLoginCredentialsDTO : IRequest<Result<UserLoginResponse>>
        {
            [EmailAddress]
            public string Email { get; set; }

            public string Password { get; set; }

        }

        public class Validator : AbstractValidator<StaffLoginCredentialsDTO>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Validator(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;

                RuleFor(c => c.Email)
                    .EmailAddress()
                    .NotEmpty();

                RuleFor(c => c.Password)
                    .NotEmpty()
                    .MinimumLength(8)
                ;
            }
        }

        internal sealed class Handler : IRequestHandler<StaffLoginCredentialsDTO, Result<UserLoginResponse>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly JWTProvider _jwtprovider;
            private readonly IValidator<StaffLoginCredentialsDTO> _validator;
            private readonly IMapper _mapper;
            public Handler(FootballAcademyDBContext dbContext, IValidator<StaffLoginCredentialsDTO> validator, JWTProvider jwtprovider, IMapper mapper)
            {
                _dbContext = dbContext;
                _validator = validator;
                _jwtprovider = jwtprovider;
                _mapper = mapper;
            }
            public async Task<Result<UserLoginResponse>> Handle(StaffLoginCredentialsDTO request, CancellationToken cancellationToken)
            {
                if (request == null)
                {
                    return Result.Failure<UserLoginResponse>(new Error("Invalid Request", "Invlaid Body Type"));
                }

                var validationResult = await _validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    return Result.Failure<UserLoginResponse>(new Error(StatusCodes.Status422UnprocessableEntity.ToString(), validationResult));
                }

                var user = await _dbContext
                    .User
                   .FirstOrDefaultAsync(x => x.Email == request.Email);

                if (user is null)
                {
                    return Result.Failure<UserLoginResponse>(new Error("BadRequest", "Invalid Email or Password"));
                }


                if (BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    var accessToken = _jwtprovider.GenerateToken(user.Id);

                    var userJoin = await _dbContext
                        .User
                        .Where(x => x.Id == user.Id)
                        .Join(_dbContext.Role.Include(r => r.Permissions),
                            u => u.RoleId,
                            r => r.Id,
                            (u, r) => new
                            {
                                Role = r,
                                User = u
                            }
                        ).Join(_dbContext.Staff,
                              ur => ur.User.Id,
                                s => s.UserId,
                                (ur, s) => new
                                {
                                    User = ur.User,
                                    Role = ur.Role,
                                    Staff = s
                                })
                    .FirstOrDefaultAsync();

                    var UserData = userJoin.User;
                    var StaffData = userJoin.Staff;
                    var UserRole = userJoin.Role;



                    var response = _mapper.Map<UserLoginResponse>(UserData);
                    response.Staff = StaffData;
                    response.Role = UserRole;
                    response.accessToken = accessToken;

                    return Result.Success<UserLoginResponse>(response);

                }

                return Result.Failure<UserLoginResponse>(new Error("NotFound", "User Not Found"));
            }


        }
    }
}

public class LoginEndpoint : ICarterModule
{
    void ICarterModule.AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/login", async (Login.StaffLoginCredentialsDTO request, ISender sender) =>
        {
            var result = await sender.Send(request);
            if (result.IsFailure)
            {
                return Results.UnprocessableEntity(result.Error);
            };
            return Results.Ok(result.Value);
        })

        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status422UnprocessableEntity))
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(StaffLoginResponseDTO), StatusCodes.Status200OK))
        .WithTags("Auth")
        ;
    }
}
