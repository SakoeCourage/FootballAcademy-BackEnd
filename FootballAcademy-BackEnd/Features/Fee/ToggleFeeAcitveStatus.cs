using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.Fee.ToggleFeeAcitveStatus;

namespace FootballAcademy_BackEnd.Features.Fee
{
    public static class ToggleFeeAcitveStatus
    {
        public class ToggleFeeActiveRequest : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<ToggleFeeActiveRequest, Result>
        {
            private readonly FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dbContext)
            {

                _dbContext = dbContext;

            }

            public async Task<Result> Handle(ToggleFeeActiveRequest request, CancellationToken cancellationToken)
            {
                var fee = await _dbContext
                    .Fee.FirstOrDefaultAsync(x => x.Id == request.Id);

                if (fee is null) return Result.Failure(Error.NotFound);

                var currentStatus = fee.IsActive;

                fee.IsActive = !currentStatus;
                fee.UpdatedAt = DateTime.UtcNow;

                return Result.Success();
            }
        }
    }
}

public class MapToggleFeeActiveStatusEdnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/fee/{Id}/toggle-active-status", async (ISender sender, Guid Id) =>
        {
            var response = await sender.Send(new ToggleFeeActiveRequest { Id = Id });

            if (response.IsFailure)
            {
                return Results.NotFound(response.Error);
            }

            return Results.NoContent();

        }).WithTags("Fee Setup")
              .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status204NoContent))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))

          ;
    }
}