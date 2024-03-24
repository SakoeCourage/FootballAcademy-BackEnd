using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.School;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.School
{
    public static class GetSchool
    {
        public class GetSchoolRequest : IRequest<Result<Entities.School>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<GetSchoolRequest, Result<Entities.School>>
        {
            FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dBContext)
            {
                _dbContext = dBContext;
            }

            public async Task<Result<Entities.School>> Handle(GetSchoolRequest request, CancellationToken cancellationToken)
            {

                var school = await _dbContext.School.FirstOrDefaultAsync(x => x.Id == request.Id);

                if (school == null)
                {
                    return Result.Failure<Entities.School>(Error.NotFound);
                }

                return Result.Success<Entities.School>(school);
            }
        }
    }
}

public class MapGetSchoolEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/school/{Id}", async (ISender sender, Guid Id) =>
        {
            var response = await sender.Send(new GetSchool.GetSchoolRequest { Id = Id });

            if (response.IsFailure)
            {
                return Results.NotFound(response.Error);
            }

            return Results.Ok(response.Value);

        }).WithTags("School")
              .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status200OK))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))

          ;
    }
}