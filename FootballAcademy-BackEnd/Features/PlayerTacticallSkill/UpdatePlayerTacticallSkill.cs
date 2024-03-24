using AutoMapper;
using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.PlayerTacticallSkill;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.PlayerTacticallSkill.UpdatePlayerTacticallSkill;

namespace FootballAcademy_BackEnd.Features.PlayerTacticallSkill
{
    public static class UpdatePlayerTacticallSkill
    {
        public class UpdatePlayerTacticalSkillRequest
        {
            public double? UnderstandingOfPositionOfPlay { get; set; }
            public double? FieldDecisionMaking { get; set; }
            public double? Awareness { get; set; }
            public double? TacticalDiscipline { get; set; }
            public double? PassingAccuracy { get; set; }
            public double? AbiityToReadGame { get; set; }
        }

        public class UpdatePlayerTacticalSkillRequestData : IRequest<Result<Guid>>
        {
            public Guid Id { get; set; }
            public Guid PlayerId { get; set; }
            public double? UnderstandingOfPositionOfPlay { get; set; }
            public double? FieldDecisionMaking { get; set; }
            public double? Awareness { get; set; }
            public double? TacticalDiscipline { get; set; }
            public double? PassingAccuracy { get; set; }
            public double? AbiityToReadGame { get; set; }
        }

        public class Validator : AbstractValidator<UpdatePlayerTacticalSkillRequestData>
        {
            public Validator()
            {
                RuleFor(c => c.UnderstandingOfPositionOfPlay).NotEmpty();
                RuleFor(c => c.FieldDecisionMaking).NotNull();
                RuleFor(c => c.TacticalDiscipline).NotNull();
                RuleFor(c => c.Awareness).NotNull();
                RuleFor(c => c.PassingAccuracy).NotNull();
                RuleFor(c => c.AbiityToReadGame).NotNull();
            }
        }

        public class Handler : IRequestHandler<UpdatePlayerTacticalSkillRequestData, Result<Guid>>
        {

            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<UpdatePlayerTacticalSkillRequestData> _validator;
            private readonly IMapper _mapper;

            public Handler(FootballAcademyDBContext dBContext, IValidator<UpdatePlayerTacticalSkillRequestData> validator, IMapper mapper)
            {
                _dbContext = dBContext;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<Result<Guid>> Handle(UpdatePlayerTacticalSkillRequestData request, CancellationToken cancellationToken)
            {

                var validationResult = _validator.Validate(request);

                if (validationResult.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResult));
                }

                var affectedRoles = await _dbContext
                    .PlayerHasTacticalSkills
                    .Where(x => x.Id == request.Id && x.PlayerId == request.PlayerId)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(p => p.UnderstandingOfPositionOfPlay, request.UnderstandingOfPositionOfPlay)
                        .SetProperty(p => p.FieldDecisionMaking, request.FieldDecisionMaking)
                        .SetProperty(p => p.TacticalDiscipline, request.TacticalDiscipline)
                        .SetProperty(p => p.Awareness, request.Awareness)
                        .SetProperty(p => p.PassingAccuracy, request.PassingAccuracy)
                        .SetProperty(p => p.AbiityToReadGame, request.AbiityToReadGame)
                        .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));

                if (affectedRoles == 0) return Result.Failure<Guid>(Error.NotFound);

                return Result.Success<Guid>(request.Id);

            }
        }
    }
}

public class UpdatePlayerTacticalSkillEndpoint : ICarterModule
{
    private readonly IMapper _mapper;
    public UpdatePlayerTacticalSkillEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/player/{PlayerId}/tactical-skill/{TacticalSkillId}", async (Guid PlayerId, Guid TacticalSkillId, UpdatePlayerTacticallSkill.UpdatePlayerTacticalSkillRequest request, ISender sender) =>
        {
            var req = _mapper.Map<UpdatePlayerTacticalSkillRequestData>(request);

            req.Id = TacticalSkillId;
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
        .WithTags("Player Tactical Skill Assessment")
        ;
        ;
    }
}

