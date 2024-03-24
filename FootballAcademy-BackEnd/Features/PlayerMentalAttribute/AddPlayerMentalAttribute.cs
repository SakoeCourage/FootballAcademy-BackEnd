using AutoMapper;
using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.PlayerMentalAttribute;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.PlayerMentalAttribute.AddPlayerMentalAttribute;

namespace FootballAcademy_BackEnd.Features.PlayerMentalAttribute
{
    public static class AddPlayerMentalAttribute
    {
        public class AddPlayerMentalAttributeRequest
        {
            public double? AttituteTowardsSport { get; set; }
            public double? Coachability { get; set; }
            public double? Confidence { get; set; }
            public double? MentalToughness { get; set; }
            public double? FocusAndConcentration { get; set; }
            public double? LeadershipQualities { get; set; }
        }

        public class AddPlayerMentalAttributeRequestData : IRequest<Result<Guid>>
        {
            public Guid PlayerId { get; set; }
            public double? AttituteTowardsSport { get; set; }
            public double? Coachability { get; set; }
            public double? Confidence { get; set; }
            public double? MentalToughness { get; set; }
            public double? FocusAndConcentration { get; set; }
            public double? LeadershipQualities { get; set; }
        }

        public class Validator : AbstractValidator<AddPlayerMentalAttributeRequestData>
        {
            public Validator()
            {
                RuleFor(c => c.AttituteTowardsSport).NotEmpty();
                RuleFor(c => c.Coachability).NotNull();
                RuleFor(c => c.Confidence).NotNull();
                RuleFor(c => c.MentalToughness).NotNull();
                RuleFor(c => c.FocusAndConcentration).NotNull();
                RuleFor(c => c.LeadershipQualities).NotNull();
            }
        }

        public class Handler : IRequestHandler<AddPlayerMentalAttributeRequestData, Result<Guid>>
        {

            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<AddPlayerMentalAttributeRequestData> _validator;
            private readonly IMapper _mapper;

            public Handler(FootballAcademyDBContext dBContext, IValidator<AddPlayerMentalAttributeRequestData> validator, IMapper mapper)
            {
                _dbContext = dBContext;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<Result<Guid>> Handle(AddPlayerMentalAttributeRequestData request, CancellationToken cancellationToken)
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

                var newData = _mapper.Map<Entities.PlayerHasMentalAttribute>(request);

                _dbContext.PlayerHasMentalAttribute.Add(newData);

                await _dbContext.SaveChangesAsync();

                return Result.Success<Guid>(newData.Id);

            }
        }
    }
}

public class AddMentalAttributeEndpoint : ICarterModule
{
    private readonly IMapper _mapper;
    public AddMentalAttributeEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/player/{Id}/mental-attribute", async (Guid Id, AddPlayerMentalAttribute.AddPlayerMentalAttributeRequest request, ISender sender) =>
        {
            var req = _mapper.Map<AddPlayerMentalAttributeRequestData>(request);

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
        .WithTags("Player Mental Attributes Assessment")
        ;
        ;
    }
}
