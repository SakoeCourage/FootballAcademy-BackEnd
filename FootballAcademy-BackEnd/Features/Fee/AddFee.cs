using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.Fee.AddFee;

namespace FootballAcademy_BackEnd.Features.Fee
{
    public static class AddFee
    {
        public class AddFeeRequest : IRequest<Result<Guid>>
        {
            public string FeeName { get; set; } = string.Empty;
            public string FeeDescription { get; set; } = string.Empty;
            public Double Amount { get; set; }
        }

        public class Validator : AbstractValidator<AddFeeRequest>
        {
            IServiceScopeFactory _serviceScopeFactory;

            public Validator(IServiceScopeFactory scopeServiceFactory)
            {
                _serviceScopeFactory = scopeServiceFactory;

                RuleFor(c => c.FeeName).NotEmpty().MustAsync(async (name, cancellation) =>
                {
                    var scope = _serviceScopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<FootballAcademyDBContext>();
                    var exist = await dbContext.Fee.AnyAsync(c => c.FeeName == name, cancellation);
                    return !exist;
                });
                RuleFor(c => c.Amount).NotEmpty();
            }
        }

        internal sealed class Hander : IRequestHandler<AddFeeRequest, Result<Guid>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<AddFeeRequest> _validator;

            public Hander(FootballAcademyDBContext dbContext, IValidator<AddFeeRequest> validator)
            {
                _dbContext = dbContext;
                _validator = validator;
            }
            public async Task<Result<Guid>> Handle(AddFeeRequest request, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request);

                if (validationResult.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResult));
                }

                var newEntry = new Entities.Fee
                {
                    FeeName = request.FeeName,
                    Amount = request.Amount,
                    IsActive = true
                };

                _dbContext.Fee.Add(newEntry);

                await _dbContext.SaveChangesAsync();

                return Result.Success<Guid>(newEntry.Id);
            }
        }
    }
}

public class MapAddFeeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/fee", async (ISender sender, AddFeeRequest request) =>
        {
            var response = await sender.Send(request);

            if (response.IsSuccess)
            {
                return Results.Ok(response.Value);
            }
            if (response.IsFailure)
            {
                return Results.UnprocessableEntity(response.Error);
            }
            return Results.BadRequest();
        }).WithTags("Fee Setup")
           .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status200OK))
           .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status422UnprocessableEntity)); ;
    }
}
