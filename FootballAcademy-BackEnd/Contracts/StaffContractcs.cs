using FootballAcademy_BackEnd.Entities;

namespace FootballAcademy_BackEnd.Contracts
{

    public record StaffLoginResponseDTO {

        public Guid Id { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public String FullName { get; set; }

        public string Phone { get; set; }
        public string? Qualification { get; set; } = String.Empty;
        public string? PassportPicture { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public virtual Role Role { get; set; }
        public string AcessToken { get; set; }
        public virtual StaffSpecialization StaffSpecialization { get; set; }
    }

    public record StaffRequestResponse {
        public String FullName { get; set; }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string? Qualification { get; set; } = String.Empty;
        public string? PassportPicture { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public virtual Role Role { get; set; }
        public virtual StaffSpecialization StaffSpecialization { get; set; }
    }
}
