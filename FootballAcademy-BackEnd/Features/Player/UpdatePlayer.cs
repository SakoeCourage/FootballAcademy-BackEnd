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
using static FootballAcademy_BackEnd.Features.Player.CreatePlayer;

namespace FootballAcademy_BackEnd.Features.Player
{
    public static class UpdatePlayer
    {
        public class UpdatePlayerRequestData : IRequest<Result<Guid>>
        {
            public Guid Id { get; set; }
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

        public class Validator : AbstractValidator<UpdatePlayerRequestData>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Validator(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
                RuleFor(c => c.Email).EmailAddress().NotEmpty()
                         .MustAsync(async (model, email, cancellation) =>
                         {
                             using (var scope = _scopeFactory.CreateScope())
                             {
                                 var dbContext = scope.ServiceProvider.GetRequiredService<FootballAcademyDBContext>();
                                 bool exists = await dbContext.Player
                                     .AnyAsync(e => e.Email.ToLower() == email.ToLower() && e.Id != model.Id);
                                 return !exists;
                             }
                         })
                         .WithMessage("Email Address Already Exist")
                ;

                RuleFor(c => c.ContactNumber).NotEmpty();
                RuleFor(c => c.FirstName).NotEmpty();
                RuleFor(c => c.SurName).NotEmpty();
                RuleFor(c => c.Gender).NotEmpty().IsInEnum();
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
        public class Hanlder : IRequestHandler<UpdatePlayerRequestData, Result<Guid>>
        {
            private readonly FootballAcademyDBContext _dbContext;
            private readonly IMapper _mapper;
            private readonly IValidator<UpdatePlayerRequestData> _validator;
            public Hanlder(FootballAcademyDBContext dBContext, IMapper mapper, IValidator<UpdatePlayerRequestData> validator)
            {
                _dbContext = dBContext;
                _mapper = mapper;
                _validator = validator;
            }

            public async Task<Result<Guid>> Handle(UpdatePlayerRequestData request, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request);

                if (validationResult.IsValid is false)
                {
                    return Result.Failure<Guid>(Error.ValidationError(validationResult));
                }

                var updateRow = await _dbContext
                    .Player
                    .Where(p => p.Id == request.Id)
                    .ExecuteUpdateAsync(setters =>
                         setters.SetProperty(p => p.PassportPicture, request.PassportPicture)
                        .SetProperty(p => p.SurName, request.SurName)
                        .SetProperty(p => p.FirstName, request.FirstName)
                        .SetProperty(p => p.Gender, request.Gender)
                        .SetProperty(p => p.OtherNames, request.OtherNames)
                        .SetProperty(p => p.DateOfBirth, request.DateOfBirth)
                        .SetProperty(p => p.ResidentialAddress, request.ResidentialAddress)
                        .SetProperty(p => p.Community, request.Community)
                        .SetProperty(p => p.ContactNumber, request.ContactNumber)
                        .SetProperty(p => p.Nationality, request.Nationality)
                        .SetProperty(p => p.Email, request.Email)
                        .SetProperty(p => p.Socials, request.Socials)
                        .SetProperty(p => p.GuardianFullName, request.GuardianFullName)
                        .SetProperty(p => p.GuardianContactNumber, request.GuardianContactNumber)
                        .SetProperty(p => p.GuardianEmail, request.GuardianEmail)
                        .SetProperty(p => p.AnyCongenitalDeformity, request.AnyCongenitalDeformity)
                        .SetProperty(p => p.CongenitalDeformityType, request.CongenitalDeformityType)
                        .SetProperty(p => p.Allergy, request.Allergy)
                        .SetProperty(p => p.BloodGroup, request.BloodGroup)
                    );

                if (updateRow == 0) return Result.Failure<Guid>(Error.NotFound);

                return Result.Success<Guid>(request.Id);
            }
        }
    }
}

public class CreateUpdateRequestEndpoint : ICarterModule
{
    private readonly IMapper _mapper;

    public CreateUpdateRequestEndpoint(IMapper mapper)
    {
        _mapper = mapper;

    }
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/player/{id}", async (Guid id, CreatePlayerRequestData request, ISender sender) =>
        {
            var req = _mapper.Map<UpdatePlayer.UpdatePlayerRequestData>(request);
            req.Id = id;
            var result = await sender.Send(req);
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

    }
}
