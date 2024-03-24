using AutoMapper;
using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.PlayerMentalAttribute;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.PlayerMentalAttribute.UpdatePlayerMentallAttribute;

namespace FootballAcademy_BackEnd.Features.PlayerMentalAttribute
{
    public static class UpdatePlayerMentallAttribute
    {
        public class UpdatePlayerMentalAttributeRequest
        {
            public double? AttituteTowardsSport { get; set; }
            public double? Coachability { get; set; }
            public double? Confidence { get; set; }
            public double? MentalToughness { get; set; }
            public double? FocusAndConcentration { get; set; }
            public double? LeadershipQualities { get; set; }
        }

        public class UpdatePlayerMentalAttributeRequestData : IRequest<Result<Guid>>
        {
            public Guid Id { get; set; }
            public Guid PlayerId { get; set; }
            public double? AttituteTowardsSport { get; set; }
            public double? Coachability { get; set; }
            public double? Confidence { get; set; }
            public double? MentalToughness { get; set; }
            public double? FocusAndConcentration { get; set; }
            public double? LeadershipQualities { get; set; }
        }

        public class Validator : AbstractValidator<UpdatePlayerMentalAttributeRequestData>
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

        public class Handler : IRequestHandler<UpdatePlayerMentalAttributeRequestData, Result<Guid>>
        {

            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<UpdatePlayerMentalAttributeRequestData> _validator;
            private readonly IMapper _mapper;

            public Handler(FootballAcademyDBContext dBContext, IValidator<UpdatePlayerMentalAttributeRequestData> validator, IMapper mapper)
            {
                _dbContext = dBContext;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<Result<Guid>> Handle(UpdatePlayerMentalAttributeRequestData request, CancellationToken cancellationToken)
            {

                var validationResult = _validator.Validate(request);

                if (validationResult.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResult));
                }

                var affectedRoles = await _dbContext
                    .PlayerHasMentalAttribute
                    .Where(x => x.Id == request.Id && x.PlayerId == request.PlayerId)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(p => p.Confidence, request.Confidence)
                        .SetProperty(p => p.AttituteTowardsSport, request.AttituteTowardsSport)
                        .SetProperty(p => p.LeadershipQualities, request.LeadershipQualities)
                        .SetProperty(p => p.MentalToughness, request.MentalToughness)
                        .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));

                if (affectedRoles == 0) return Result.Failure<Guid>(Error.NotFound);

                return Result.Success<Guid>(request.Id);

            }
        }
    }
}

public class UpdateMentalAttributeEndpoint : ICarterModule
{
    private readonly IMapper _mapper;
    public UpdateMentalAttributeEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/player/{PlayerId}/mental-attribute/{MentalAttributeId}", async (Guid PlayerId, Guid MentalAttributeId, UpdatePlayerMentallAttribute.UpdatePlayerMentalAttributeRequest request, ISender sender) =>
        {
            var req = _mapper.Map<UpdatePlayerMentalAttributeRequestData>(request);

            req.Id = MentalAttributeId;
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
        .WithTags("Player Mental Attributes Assessment")
        ;
        ;
    }
}


