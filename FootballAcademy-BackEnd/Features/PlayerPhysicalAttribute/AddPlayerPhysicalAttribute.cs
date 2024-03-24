using AutoMapper;
using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.PlayerPhysicalAttribute;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.PlayerPhysicalAttribute.AddPlayerPhysicalAttribute;

namespace FootballAcademy_BackEnd.Features.PlayerPhysicalAttribute
{
    public static class AddPlayerPhysicalAttribute
    {
        public class AddPlayerPhysicalAttributeRequest
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

        public class AddPlayerPhysicalAttributeRequestData : IRequest<Result<Guid>>
        {
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

        public class Validator : AbstractValidator<AddPlayerPhysicalAttributeRequestData>
        {
            public Validator()
            {
                RuleFor(c => c.PlayerId).NotEmpty();
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

        public class Handler : IRequestHandler<AddPlayerPhysicalAttributeRequestData, Result<Guid>>
        {

            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<AddPlayerPhysicalAttributeRequestData> _validator;
            private readonly IMapper _mapper;

            public Handler(FootballAcademyDBContext dBContext, IValidator<AddPlayerPhysicalAttributeRequestData> validator, IMapper mapper)
            {
                _dbContext = dBContext;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<Result<Guid>> Handle(AddPlayerPhysicalAttributeRequestData request, CancellationToken cancellationToken)
            {
                var playerExist = await _dbContext.Player.AnyAsync(p => p.Id == request.PlayerId);

                if (playerExist is false)
                {
                    return Result.Failure<Guid>(Error.NotFound);
                }

                var validationResult = _validator.Validate(request);

                if (validationResult.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResult));
                }

                var newData = _mapper.Map<Entities.PlayerHasPhysicalAttribute>(request);

                _dbContext.PlayerHasPhysicalAttribute.Add(newData);

                await _dbContext.SaveChangesAsync();

                return Result.Success<Guid>(newData.Id);

            }
        }
    }
}

public class AddPhysicalAttributeEndpoint : ICarterModule
{
    private readonly IMapper _mapper;
    public AddPhysicalAttributeEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/player/{Id}/physical-attribute", async (Guid Id, AddPlayerPhysicalAttribute.AddPlayerPhysicalAttributeRequest request, ISender sender) =>
        {
            var req = _mapper.Map<AddPlayerPhysicalAttributeRequestData>(request);

            req.PlayerId = Id;

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
