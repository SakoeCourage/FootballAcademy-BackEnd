using AutoMapper;
using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Features.Player;
using FootballAcademy_BackEnd.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FootballAcademy_BackEnd.Features.Player
{
    public static class CreatePlayer
    {
        public class CreatePlayerRequestData : IRequest<Result<Guid>>
        {
            public string? PassportPicture { get; set; }
            public string SurName { get; set; } = String.Empty;
            public string FirstName { get; set; } = String.Empty;
            public string? Gender { get; set; }
            public string? OtherNames { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string? ResidentialAddress { get; set; }
            public string? Community { get; set; }
            public string? CurrentSchool { get; set; }
            public string? CurrentClass { get; set; }
            public string? ContactNumber { get; set; }
            public string? Nationality { get; set; }
            [EmailAddress]
            public string? Email { get; set; }
            public string? Socials { get; set; }
            public string GuardianFullName { get; set; } = String.Empty;
            [EmailAddress]
            public string? GuardianEmail { get; set; } = String.Empty;
            public string GuardianContactNumber { get; set; } = String.Empty;
            public Boolean AnyCongenitalDeformity { get; set; } = false;
            public string? CongenitalDeformityType { get; set; }
            public string? BloodGroup { get; set; }
            public string? Allergy { get; set; }
        }
        public class Validator : AbstractValidator<CreatePlayerRequestData>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Validator(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
                RuleFor(c => c.Email).EmailAddress().NotEmpty()
                         .MustAsync(async (email, cancellation) =>
                         {
                             using (var scope = _scopeFactory.CreateScope())
                             {
                                 var dbContext = scope.ServiceProvider.GetRequiredService<FootballAcademyDBContext>();
                                 bool exists = await dbContext.Player
                                     .AnyAsync(e => e.Email.ToLower() == email.ToLower());
                                 return !exists;
                             }
                         })
                         .WithMessage("Email Address Already Exist")
                ;

                RuleFor(c => c.ContactNumber).NotEmpty();
                RuleFor(c => c.FirstName).NotEmpty();
                RuleFor(c => c.SurName).NotEmpty();
                RuleFor(c => c.Gender).NotEmpty();
                RuleFor(c => c.GuardianFullName).NotEmpty();
                RuleFor(c => c.GuardianContactNumber).NotEmpty();

                RuleFor(c => c.AnyCongenitalDeformity).NotEmpty();
                RuleFor(c => c.CongenitalDeformityType)
              .Must((model, type) =>
              {
                  return !model.AnyCongenitalDeformity || type != null;
              })
              .WithMessage("Congenital Deformity Type cannot be null when there is AnyCongenitalDeformity");

            }
        }

        public class Hanlder : IRequestHandler<CreatePlayerRequestData, Result<Guid>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IMapper _mapper;
            private readonly IValidator<CreatePlayerRequestData> _validator;
            public Hanlder(FootballAcademyDBContext dBContext, IMapper mapper, IValidator<CreatePlayerRequestData> validator)
            {
                _dbContext = dBContext;
                _mapper = mapper;
                _validator = validator;
            }
            public async Task<Result<Guid>> Handle(CreatePlayerRequestData request, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request);

                if (validationResult.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResult));
                }

                var playerData = _mapper.Map<Entities.Player>(request);


                _dbContext.Add(playerData);
                await _dbContext.SaveChangesAsync();

                return Result.Success<Guid>(playerData.Id);


            }
        }
    }
}

public class CreatePlayerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/player", async (CreatePlayer.CreatePlayerRequestData request, ISender sender) =>
        {
            var result = await sender.Send(request);
            if (result.IsFailure)
            {
                return Results.UnprocessableEntity(result.Error);
            };

            return Results.Ok(result.Value);
        })
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Error), StatusCodes.Status422UnprocessableEntity))
        .WithMetadata(new ProducesResponseTypeAttribute(typeof(Guid), StatusCodes.Status200OK))
        .WithTags("Player Registeration")
        ;
        ;
    }
}
