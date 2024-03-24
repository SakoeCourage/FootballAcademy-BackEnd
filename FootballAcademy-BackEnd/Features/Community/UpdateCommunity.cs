using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.Community;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.Community.UpdateCommunity;

namespace FootballAcademy_BackEnd.Features.Community
{
    public class UpdateCommunity
    {
        public class UpdateCommunityRequest
        {
            public string CommunityName { get; set; }
            public string District { get; set; }
        }

        public class UpdateCommunityRequestData : IRequest<Result<Guid>>
        {
            public Guid Id { get; set; }
            public string CommunityName { get; set; }
            public string District { get; set; }
        }

        public class Validator : AbstractValidator<UpdateCommunityRequestData>
        {
            private readonly IServiceScopeFactory _scopeFactory;
            public Validator(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
                RuleFor(c => c.CommunityName).NotEmpty()
                    .MustAsync(async (model, name, cancellationToken) =>
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetService<FootballAcademyDBContext>();
                            var exist = await dbContext.Community.AnyAsync(s => s.CommunityName == name && s.Id != model.Id, cancellationToken);
                            return !exist;
                        }
                    }).WithMessage("Community Name Already Exist")
                    ;

            }
        }
        public class Handler : IRequestHandler<UpdateCommunityRequestData, Result<Guid>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<UpdateCommunityRequestData> _validator;
            public Handler(FootballAcademyDBContext dbContext, IValidator<UpdateCommunityRequestData> validator)
            {

                _dbContext = dbContext;
                _validator = validator;

            }
            public async Task<Result<Guid>> Handle(UpdateCommunityRequestData request, CancellationToken cancellationToken)
            {
                var validationResponse = await _validator.ValidateAsync(request, cancellationToken);

                if (validationResponse.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResponse));
                }

                var affectedColumns = await _dbContext.Community.ExecuteUpdateAsync(setters =>
                        setters.SetProperty(p => p.CommunityName, request.CommunityName)
                        .SetProperty(p => p.District, request.District)
                        .SetProperty(p => p.UpdatedAt, DateTime.UtcNow)
                );
                if (affectedColumns == 0) return Result.Failure<Guid>(Error.NotFound);

                return Result.Success<Guid>(request.Id);
            }
        }
    }
}


public class MapUpdateCommunityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/community/{Id}", async (ISender sender, Guid Id, UpdateCommunity.UpdateCommunityRequest request) =>
        {
            var response = await sender.Send(
                new UpdateCommunityRequestData
                {
                    Id = Id,
                    CommunityName = request.CommunityName,
                    District = request.District
                }
                );

            if (response.IsFailure)
            {
                return Results.UnprocessableEntity(response.Error);
            }

            return Results.Ok(response.Value);

        }).WithTags("Community")
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Guid), StatusCodes.Status200OK))
              .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status400BadRequest));
    }
}

