using Carter;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.Fee.DeleteFee;

namespace FootballAcademy_BackEnd.Features.Fee
{
    public static class DeleteFee
    {
        public class DeleteFeeRequest : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<DeleteFeeRequest, Result>
        {
            private readonly FootballAcademyDBContext _dbContext;
            public Handler(FootballAcademyDBContext dbContext)
            {

                _dbContext = dbContext;

            }

            public async Task<Result> Handle(DeleteFeeRequest request, CancellationToken cancellationToken)
            {
                var affectedRows = await _dbContext
                    .Fee
                    .Where(s => s.Id == request.Id)
                    .ExecuteDeleteAsync();

                if (affectedRows == 0) return Result.Failure(Error.NotFound);
                return Result.Success();
            }
        }
    }
}

public class MapDeleteFeeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/fee/{Id}", async (ISender sender, Guid Id) =>
        {
            var response = await sender.Send(new DeleteFeeRequest { Id = Id });

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
