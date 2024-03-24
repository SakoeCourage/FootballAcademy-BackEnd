using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.StaffSpecialization;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Features.StaffSpecialization
{
    public static class DeleteStaffSpecilization
    {
        public class DeleteRequest : IRequest<Result<string>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<DeleteRequest, Result<string>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dBContext)
            {
                _dbContext = dBContext;

            }
            public async Task<Result<string>> Handle(DeleteRequest request, CancellationToken cancellationToken)
            {
                var staffSpecilization = await _dbContext.StaffSpecialization.FirstOrDefaultAsync(x => x.Id == request.Id);

                if (staffSpecilization == null)
                {
                    return Result.Failure<string>(Error.NotFound);
                }

                _dbContext.StaffSpecialization.Remove(staffSpecilization);

                await _dbContext.SaveChangesAsync();

                return Result.Success<string>("Staff Specialization Removed Successfully");
            }
        }
    }

}

public class DeleteStaffSpecializationEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/staff-specialization/{Id}", async (Guid Id, ISender sender) =>
        {
            var response = await sender.Send(new DeleteStaffSpecilization.DeleteRequest
            {
                Id = Id
            });

            if (response.IsFailure)
            {
                return Results.BadRequest(response.Error);
            }
            if (response.IsSuccess)
            {
                return Results.Ok(response?.Value);
            }
            return Results.BadRequest("Something went wrong");

        }).WithTags("Role")
              .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status200OK))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))
              .WithTags("Staff Specialization")

        ;
    }
}

