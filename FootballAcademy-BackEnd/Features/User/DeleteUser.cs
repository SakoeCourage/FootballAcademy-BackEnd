using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.User;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.User
{
    public static class DeleteUser
    {

        public class UserDeleteRequest : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        internal sealed class Handler : IRequestHandler<UserDeleteRequest, Result>
        {
            FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<Result> Handle(UserDeleteRequest request, CancellationToken cancellationToken)
            {
                if (request == null)
                {
                    return (Result)Result.InvalidRequest();
                }
                var user = await _dbContext.User.FindAsync(request.Id);

                if (user is null)
                {
                    return (Result)Result.InvalidRequest();
                }

                _dbContext.User.Remove(user);

                try
                {
                    await _dbContext.SaveChangesAsync();
                    return (Result)Result.Success("Ok");
                }
                catch (DbUpdateException ex)
                {
                    return Result.Failure(new Error("500", "Error deleting user: " + ex.Message));
                }
            }
        }

    }
}

public class DeletUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/user/{id}", async (Guid id, ISender sender) =>
        {
            var response = await sender.Send(
                new DeleteUser.UserDeleteRequest
                {
                    Id = id
                }
                );

            if (response.IsFailure)
            {
                return Results.UnprocessableEntity(response.Error);
            }
            return Results.NoContent();
        }).WithTags("User")
            ;
    }
}
