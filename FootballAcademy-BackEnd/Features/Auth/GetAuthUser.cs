using AutoMapper;
using Carter;
using FootballAcademy_BackEnd.Contracts;
using FootballAcademy_BackEnd.Providers;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static FootballAcademy_BackEnd.Features.Auth.GetAuthUser;


namespace FootballAcademy_BackEnd.Features.Auth
{
    public class GetAuthUser
    {
        public class GetAuthUserQuery : IRequest<Result<GetUserResponse?>>
        {
        }

        internal sealed class Handler : IRequestHandler<GetAuthUserQuery, Result<GetUserResponse?>>
        {
            private readonly Authprovider _authProvider;
            private readonly IMapper _mapper;
            public Handler(Authprovider authProvider, IMapper mapper)
            {
                _authProvider = authProvider;
                _mapper = mapper;
            }
            public async Task<Result<GetUserResponse?>> Handle(GetAuthUserQuery query, CancellationToken cancellationToken)
            {
                var user = await _authProvider.GetAuthUser();

                if (user is null)
                    return Result.Failure<GetUserResponse?>(new Error(StatusCodes.Status400BadRequest.ToString(), "User not found"));

                return Result.Success<GetUserResponse?>(user);
            }
        }
    }
}

public class getAuthUserEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/auth/user",

        [Authorize]
        async (ISender sender) =>
        {
            var response = await sender.Send(new GetAuthUserQuery());

            if (response is null)
            {
                return Results.Unauthorized();
            }

            if (response.IsSuccess)
            {
                return Results.Ok(response?.Value);
            }

            return Results.Unauthorized();
        })
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(GetUserResponse), StatusCodes.Status200OK))
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status401Unauthorized))
            .WithTags("Auth")
            ;
    }
}
