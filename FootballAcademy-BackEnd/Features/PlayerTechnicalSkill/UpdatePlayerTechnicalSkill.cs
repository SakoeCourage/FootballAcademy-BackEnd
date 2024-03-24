using AutoMapper;
using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.PlayerTechnicalSkill.UpdatePlayerTechnicalSkill;

namespace FootballAcademy_BackEnd.Features.PlayerTechnicalSkill
{
    public static class UpdatePlayerTechnicalSkill
    {
        public class UpdatePlayerTechnicalSkillRequest
        {
            public double? PassingAccuracy { get; set; }
            public double? ShootingAccuracy { get; set; }
            public double? DribblingAbility { get; set; }
            public double? BallControll { get; set; }
            public double? HeadingAbility { get; set; }
            public double? TacklingAbility { get; set; }
            public double? CrossingAbility { get; set; }
            public double? SetPieceProficiency { get; set; }
            public double? WeakFootProficiency { get; set; }
        }

        public class UpdatePlayerTechnicalSkillRequestData : IRequest<Result<Guid>>
        {
            public Guid Id { get; set; }
            public Guid PlayerId { get; set; }
            public double? PassingAccuracy { get; set; }
            public double? ShootingAccuracy { get; set; }
            public double? DribblingAbility { get; set; }
            public double? BallControll { get; set; }
            public double? HeadingAbility { get; set; }
            public double? TacklingAbility { get; set; }
            public double? CrossingAbility { get; set; }
            public double? SetPieceProficiency { get; set; }
            public double? WeakFootProficiency { get; set; }
        }

        public class Validator : AbstractValidator<UpdatePlayerTechnicalSkillRequestData>
        {
            public Validator()
            {
                RuleFor(c => c.PassingAccuracy).NotEmpty();
                RuleFor(c => c.ShootingAccuracy).NotNull();
                RuleFor(c => c.DribblingAbility).NotNull();
                RuleFor(c => c.BallControll).NotNull();
                RuleFor(c => c.HeadingAbility).NotNull();
                RuleFor(c => c.TacklingAbility).NotNull();
                RuleFor(c => c.CrossingAbility).NotNull();
                RuleFor(c => c.SetPieceProficiency).NotNull();
                RuleFor(c => c.WeakFootProficiency).NotNull();
            }
        }

        public class Handler : IRequestHandler<UpdatePlayerTechnicalSkillRequestData, Result<Guid>>
        {

            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<UpdatePlayerTechnicalSkillRequestData> _validator;
            private readonly IMapper _mapper;

            public Handler(FootballAcademyDBContext dBContext, IValidator<UpdatePlayerTechnicalSkillRequestData> validator, IMapper mapper)
            {
                _dbContext = dBContext;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<Result<Guid>> Handle(UpdatePlayerTechnicalSkillRequestData request, CancellationToken cancellationToken)
            {

                var validationResult = _validator.Validate(request);

                if (validationResult.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResult));
                }

                var affectedRoles = await _dbContext
                    .PlayerHasTechnicalSkills
                    .Where(x => x.Id == request.Id && x.PlayerId == request.PlayerId)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(p => p.PassingAccuracy, request.PassingAccuracy)
                        .SetProperty(p => p.ShootingAccuracy, request.ShootingAccuracy)
                        .SetProperty(p => p.DribblingAbility, request.DribblingAbility)
                        .SetProperty(p => p.BallControll, request.BallControll)
                        .SetProperty(p => p.HeadingAbility, request.HeadingAbility)
                        .SetProperty(p => p.TacklingAbility, request.TacklingAbility)
                        .SetProperty(p => p.CrossingAbility, request.CrossingAbility)
                        .SetProperty(p => p.SetPieceProficiency, request.SetPieceProficiency)
                        .SetProperty(p => p.WeakFootProficiency, request.WeakFootProficiency)
                        .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));

                if (affectedRoles == 0) return Result.Failure<Guid>(Error.NotFound);

                return Result.Success<Guid>(request.Id);

            }
        }
    }
}

public class UpdatePlayerTechnicalSkillEndpoint : ICarterModule
{
    private readonly IMapper _mapper;
    public UpdatePlayerTechnicalSkillEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/player/{PlayerId}/technical-skill/{TechnicalSkillId}", async (Guid PlayerId, Guid TechnicalSkillId, UpdatePlayerTechnicalSkillRequest request, ISender sender) =>
        {
            var req = _mapper.Map<UpdatePlayerTechnicalSkillRequestData>(request);

            req.Id = TechnicalSkillId;
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
        .WithTags("Player Technical Skill Assessment")
        ;
        ;
    }
}

