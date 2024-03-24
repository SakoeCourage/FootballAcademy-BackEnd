using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using static FootballAcademy_BackEnd.Features.Community.AddCommunity;

namespace FootballAcademy_BackEnd.Features.Community
{
    public static class AddCommunity
    {
        public class AddComunityRequest : IRequest<Result<Guid>>
        {
            public string CommunityName { get; set; }
            public string District { get; set; }
        }

        public class Validator : AbstractValidator<AddComunityRequest>
        {
            private readonly IServiceScopeFactory _scopeFactory;
            public Validator(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
                RuleFor(c => c.CommunityName).NotEmpty()
                    .MustAsync(async (name, cancellationToken) =>
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetService<FootballAcademyDBContext>();
                            var exist = await dbContext.Community.AnyAsync(s => s.CommunityName == name, cancellationToken);
                            return !exist;
                        }
                    }).WithMessage("Community Name Already Exist")
                    ;

            }
        }

        public class Hanlder : IRequestHandler<AddComunityRequest, Result<Guid>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<AddComunityRequest> _validator;
            public Hanlder(FootballAcademyDBContext dbContext, IValidator<AddComunityRequest> validator)
            {

                _dbContext = dbContext;
                _validator = validator;

            }
            public async Task<Result<Guid>> Handle(AddComunityRequest request, CancellationToken cancellationToken)
            {
                var validationResponse = await _validator.ValidateAsync(request, cancellationToken);

                if (validationResponse.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResponse));
                }

                var newCommunity = new Entities.Community
                {
                    CommunityName = request.CommunityName,
                    District = request.District
                };

                _dbContext.Community.Add(newCommunity);

                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbException ex)
                {
                    return Result.Failure<Guid>(Error.BadRequest(ex.Message));
                }
                return Result.Success<Guid>(newCommunity.Id);
            }
        }
    }
}


public class MapAddCommunityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/community", async (ISender sender, AddComunityRequest request) =>
        {
            var response = await sender.Send(request);
            if (response.IsFailure)
            {
                return Results.UnprocessableEntity(response.Error);
            }

            return Results.Ok(response.Value);

        }).WithTags("Community")
              .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status200OK))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest))

          ;
    }
}
