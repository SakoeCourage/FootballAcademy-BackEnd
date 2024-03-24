using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.Fee;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.Fee.UpdateFee;

namespace FootballAcademy_BackEnd.Features.Fee
{
    public static class UpdateFee
    {
        public class UpdateFeeRequest
        {
            public string FeeName { get; set; } = string.Empty;
            public string FeeDescription { get; set; } = string.Empty;
            public Double Amount { get; set; }
        }
        public class UpdateFeeRequestData : IRequest<Result<Guid>>
        {
            public Guid Id { get; set; }
            public string FeeName { get; set; } = string.Empty;
            public string FeeDescription { get; set; } = string.Empty;
            public Double Amount { get; set; }
        }

        public class Validator : AbstractValidator<UpdateFeeRequestData>
        {
            IServiceScopeFactory _serviceScopeFactory;

            public Validator(IServiceScopeFactory scopeServiceFactory)
            {
                _serviceScopeFactory = scopeServiceFactory;

                RuleFor(c => c.FeeName).NotEmpty().MustAsync(async (model, name, cancellation) =>
                {
                    var scope = _serviceScopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<FootballAcademyDBContext>();
                    var exist = await dbContext.Fee.AnyAsync(c => c.FeeName == name && c.Id != model.Id, cancellation);
                    return !exist;
                });
                RuleFor(c => c.Amount).NotEmpty();
            }
        }

        internal sealed class Hander : IRequestHandler<UpdateFeeRequestData, Result<Guid>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<UpdateFeeRequestData> _validator;

            public Hander(FootballAcademyDBContext dbContext, IValidator<UpdateFeeRequestData> validator)
            {
                _dbContext = dbContext;
                _validator = validator;
            }

            public async Task<Result<Guid>> Handle(UpdateFeeRequestData request, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request);

                if (validationResult.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResult));
                }

                var affectedRows = await _dbContext.Fee.Where(f => f.Id == request.Id)
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(p => p.FeeDescription, request.FeeDescription)
                            .SetProperty(p => p.FeeName, request.FeeName)
                            .SetProperty(p => p.Amount, request.Amount)
                            .SetProperty(p => p.UpdatedAt, DateTime.UtcNow)
                        );
                if (affectedRows == 0) return Result.Failure<Guid>(Error.NotFound);

                return Result.Success(request.Id);
            }
        }

    }
}

public class MapUpdateFeeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/fee/{Id}", async (ISender sender, Guid Id, UpdateFee.UpdateFeeRequest request) =>
        {
            var response = await sender.Send(
                new UpdateFeeRequestData
                {
                    Id = Id,
                    FeeName = request.FeeName,
                    FeeDescription = request.FeeDescription,
                    Amount = request.Amount
                }
                );

            if (response.IsFailure)
            {
                return Results.UnprocessableEntity(response.Error);
            }

            return Results.Ok(response.Value);

        }).WithTags("Fee Setup")
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Guid), StatusCodes.Status200OK))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest));
    }
}
