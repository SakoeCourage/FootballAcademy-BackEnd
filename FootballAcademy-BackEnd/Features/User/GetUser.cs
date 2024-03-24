using AutoMapper;
using Carter;
using FootballAcademy_BackEnd.Contracts;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.User;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.User
{
    public static class GetUser
    {
        public class GetUserRequest : IRequest<Result<GetUserResponse>>
        {
            public Guid Id { get; set; }
        }

        internal sealed class Handler : IRequestHandler<GetUserRequest, Result<GetUserResponse>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IMapper _mapper;
            public Handler(FootballAcademyDBContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }
            public async Task<Result<GetUserResponse>> Handle(GetUserRequest request, CancellationToken cancellationToken)
            {
                if (request == null)
                {
                    return Result.Failure<GetUserResponse>(Error.NotFound);
                }



                var userQuery = await _dbContext.User.Where(x => x.Id == request.Id).Include(u => u.Staff)
                   .ThenInclude(s => s.StaffSpecialization)
                   .Include(u => u.Role)
                   .ThenInclude(u => u.Permissions)
                   .ToListAsync();

                if (userQuery.Count == 0)
                {
                    return Result.Failure<GetUserResponse>(Error.NotFound);
                }

                var response = _mapper.Map<GetUserResponse>(userQuery[0]);

                return Result.Success(response);

            }
        }

    }
}

public class GetUserEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/user/{id}",
        async (Guid id, ISender sender) =>
        {
            var response = await sender.Send(new GetUser.GetUserRequest
            {
                Id = id
            });

            if (response.IsSuccess)
            {
                return Results.Ok(response?.Value);
            }
            if (response.IsFailure)
            {
                return Results.Ok(response.Error);
            }


            return Results.BadRequest();
        })
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(GetAuthUserResponse), StatusCodes.Status200OK))
            .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status401Unauthorized))
            .WithTags("User")
            ;
    }
}
