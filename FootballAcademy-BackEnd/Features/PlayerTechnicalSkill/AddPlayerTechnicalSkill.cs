using AutoMapper;
using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.PlayerTechnicalSkill;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FootballAcademy_BackEnd.Features.PlayerTechnicalSkill.AddPlayerTechnicalSkill;

namespace FootballAcademy_BackEnd.Features.PlayerTechnicalSkill
{
    public static class AddPlayerTechnicalSkill
    {
        public class AddTechnicalSkillRequest
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

        public class AddTechnicalSkillRequestData : IRequest<Result<Guid>>
        {
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

        public class Validator : AbstractValidator<AddTechnicalSkillRequestData>
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

        public class Handler : IRequestHandler<AddTechnicalSkillRequestData, Result<Guid>>
        {

            private readonly FootballAcademyDBContext _dbContext;
            private readonly IValidator<AddTechnicalSkillRequestData> _validator;
            private readonly IMapper _mapper;

            public Handler(FootballAcademyDBContext dBContext, IValidator<AddTechnicalSkillRequestData> validator, IMapper mapper)
            {
                _dbContext = dBContext;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<Result<Guid>> Handle(AddTechnicalSkillRequestData request, CancellationToken cancellationToken)
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

                var newData = _mapper.Map<Entities.PlayerHasTechnicalSkills>(request);

                _dbContext.PlayerHasTechnicalSkills.Add(newData);

                await _dbContext.SaveChangesAsync();

                return Result.Success<Guid>(newData.Id);

            }
        }
    }

}

public class AddTechnicalSkillEndpoint : ICarterModule
{
    private readonly IMapper _mapper;
    public AddTechnicalSkillEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/player/{Id}/technical-skill", async (Guid Id, AddPlayerTechnicalSkill.AddTechnicalSkillRequest request, ISender sender) =>
        {
            var req = _mapper.Map<AddTechnicalSkillRequestData>(request);

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
        .WithTags("Player Technical Skill Assessment")
        ;
        ;
    }
}
