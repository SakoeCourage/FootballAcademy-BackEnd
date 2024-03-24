using FootballAcademy_BackEnd.Entities;
using System.ComponentModel.DataAnnotations;

namespace FootballAcademy_BackEnd.Contracts
{

    public record LoginResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string accessToken { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public record GetAuthUserResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Staff Staff { get; set; }
        public Role Role { get; set; }
    }

    public record UserLoginResponse
    {
        public Guid Id { get; set; }
        public string accessToken { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Staff Staff { get; set; }
        public Role Role { get; set; }
    }

    public record UserWithoutPassword
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public record GetUserResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Staff Staff { get; set; }
        public Role Role { get; set; }
    }


    public record UpdateUserRequest
    {
        public string UserName { get; set; } = String.Empty;
        [EmailAddress]
        public string Email { get; set; } = String.Empty;
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; }
        public string? OtherNames { get; set; }
        public Guid StaffSpecializationId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public Guid RoleId { get; set; }
        public string? Qualification { get; set; } = String.Empty;
        public string? PassportPicture { get; set; }

    }
}
