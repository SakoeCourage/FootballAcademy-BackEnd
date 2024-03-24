using AutoMapper;
using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.PlayerTacticallSkill;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.PlayerTacticallSkill.AddPlayerTacticalSkill;

namespace FootballAcademy_BackEnd.Features.PlayerTacticallSkill
{
    public static class AddPlayerTacticalSkill
    {
        public class AddTacticalSkillRequest
        {
            public double? UnderstandingOfPositionOfPlay { get; set; }
            public double? FieldDecisionMaking { get; set; }
            public double? Awareness { get; set; }
            public double? TacticalDiscipline { get; set; }
            public double? PassingAccuracy { get; set; }
            public double? AbiityToReadGame { get; set; }
        }

        public class AddTacticalSkillRequestData : IRequest<Result<Guid>>
        {
            public Guid PlayerId { get; set; }
            public double? UnderstandingOfPositionOfPlay { get; set; }
            public double? FieldDecisionMaking { get; set; }
            public double? Awareness { get; set; }
            public double? TacticalDiscipline { get; set; }
            public double? PassingAccuracy { get; set; }
            public double? AbiityToReadGame { get; set; }
        }

        public class Validator : AbstractValidator<AddTacticalSkillRequestData>
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

        public class Handler : IRequestHandler<AddTacticalSkillRequestData, Result<Guid>>
        {

            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<AddTacticalSkillRequestData> _validator;
            private readonly IMapper _mapper;

            public Handler(FootballAcademyDBContext dBContext, IValidator<AddTacticalSkillRequestData> validator, IMapper mapper)
            {
                _dbContext = dBContext;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<Result<Guid>> Handle(AddTacticalSkillRequestData request, CancellationToken cancellationToken)
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

                var newData = _mapper.Map<Entities.PlayerHasTacticalSkills>(request);

                _dbContext.PlayerHasTacticalSkills.Add(newData);

                await _dbContext.SaveChangesAsync();

                return Result.Success<Guid>(newData.Id);

            }
        }
    }

}

public class AddTacticalSkillEndpoint : ICarterModule
{
    private readonly IMapper _mapper;
    public AddTacticalSkillEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/player/{Id}/tactical-skill", async (Guid Id, AddPlayerTacticalSkill.AddTacticalSkillRequest request, ISender sender) =>
        {
            var req = _mapper.Map<AddTacticalSkillRequestData>(request);

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
        .WithTags("Player Tactical Skill Assessment");
    }
}
