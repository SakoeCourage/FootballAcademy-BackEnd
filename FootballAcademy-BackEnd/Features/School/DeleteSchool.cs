using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.School;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.School
{
    public static class DeleteSchool
    {

        public class DeleteSchoolRequest : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<DeleteSchoolRequest, Result>
        {
            private readonly FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dbContext)
            {

                _dbContext = dbContext;

            }

            public async Task<Result> Handle(DeleteSchoolRequest request, CancellationToken cancellationToken)
            {
                var affectedRows = await _dbContext
                    .School
                    .Where(s => s.Id == request.Id)
                    .ExecuteDeleteAsync();

                if (affectedRows == 0) return Result.Failure(Error.NotFound);
                return Result.Success();
            }
        }
    }
}

public class MapDeleteSchoolEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/school/{Id}", async (ISender sender, Guid Id) =>
        {
            var response = await sender.Send(new DeleteSchool.DeleteSchoolRequest { Id = Id });

            if (response.IsFailure)
            {
                return Results.NotFound(response.Error);
            }

            return Results.NoContent();

        }).WithTags("School")
              .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status204NoContent))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))

          ;
    }
}
