using AutoMapper;
using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.PlayerPhysicalAttribute;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.PlayerPhysicalAttribute.UpdatePlayerPhysicalAttribute;

namespace FootballAcademy_BackEnd.Features.PlayerPhysicalAttribute
{
    public static class UpdatePlayerPhysicalAttribute
    {
        public class UpdatePlayerPhysicalAttributeRequest
        {
            public double Height { get; set; }
            public double Weight { get; set; }
            public string DominantFoot { get; set; }
            public double Speed { get; set; }
            public double Endurance { get; set; }
            public double Strength { get; set; }
            public double Agility { get; set; }
            public double Flexibility { get; set; }
        }

        public class UpdatePlayerPhysicalAttributeRequestData : IRequest<Result<Guid>>
        {
            public Guid Id { get; set; }
            public Guid PlayerId { get; set; }
            public double Height { get; set; }
            public double Weight { get; set; }
            public string DominantFoot { get; set; }
            public double Speed { get; set; }
            public double Endurance { get; set; }
            public double Strength { get; set; }
            public double Agility { get; set; }
            public double Flexibility { get; set; }
        }

        public class Validator : AbstractValidator<UpdatePlayerPhysicalAttributeRequestData>
        {
            public Validator()
            {
                RuleFor(c => c.Id).NotEmpty();
                RuleFor(c => c.Height).NotNull();
                RuleFor(c => c.Weight).NotNull();
                RuleFor(c => c.DominantFoot).NotNull();
                RuleFor(c => c.Speed).NotNull();
                RuleFor(c => c.Endurance).NotNull();
                RuleFor(c => c.Strength).NotNull();
                RuleFor(c => c.Agility).NotNull();
                RuleFor(c => c.Flexibility).NotNull();
            }
        }

        public class Handler : IRequestHandler<UpdatePlayerPhysicalAttributeRequestData, Result<Guid>>
        {

            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<UpdatePlayerPhysicalAttributeRequestData> _validator;
            private readonly IMapper _mapper;

            public Handler(FootballAcademyDBContext dBContext, IValidator<UpdatePlayerPhysicalAttributeRequestData> validator, IMapper mapper)
            {
                _dbContext = dBContext;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<Result<Guid>> Handle(UpdatePlayerPhysicalAttributeRequestData request, CancellationToken cancellationToken)
            {

                var validationResult = _validator.Validate(request);

                if (validationResult.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResult));
                }

                var affectedRoles = await _dbContext
                    .PlayerHasPhysicalAttribute
                    .Where(x => x.Id == request.Id && x.PlayerId == request.PlayerId)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(p => p.Endurance, request.Endurance)
                        .SetProperty(p => p.Agility, request.Agility)
                        .SetProperty(p => p.Height, request.Height)
                        .SetProperty(p => p.Weight, request.Weight)
                        .SetProperty(p => p.DominantFoot, request.DominantFoot)
                        .SetProperty(p => p.Speed, request.Speed)
                        .SetProperty(p => p.Strength, request.Strength)
                        .SetProperty(p => p.Flexibility, request.Flexibility)
                        .SetProperty(p => p.UpdatedAt, DateTime.UtcNow)
                        );

                if (affectedRoles == 0) return Result.Failure<Guid>(Error.NotFound);

                return Result.Success<Guid>(request.Id);

            }
        }
    }
}

public class UpdatePhysicalAttributeEndpoint : ICarterModule
{
    private readonly IMapper _mapper;
    public UpdatePhysicalAttributeEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/player/{PlayerId}/physical-attribute/{PysicalAttributeId}", async (Guid PlayerId, Guid PysicalAttributeId, UpdatePlayerPhysicalAttribute.UpdatePlayerPhysicalAttributeRequest request, ISender sender) =>
        {
            var req = _mapper.Map<UpdatePlayerPhysicalAttributeRequestData>(request);

            req.Id = PysicalAttributeId;
            req.PlayerId = PlayerId;

            var result = await sender.Send(req);

            if (result.IsFailure)
            {
                return Results.UnprocessableEntity(result.Error);
            };

            return Results.Ok(result.Value);
        })
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status422UnprocessableEntity))
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Guid), StatusCodes.Status200OK))
        .WithTags("Player Physical Attributes Assessment")
        ;
        ;
    }
}


