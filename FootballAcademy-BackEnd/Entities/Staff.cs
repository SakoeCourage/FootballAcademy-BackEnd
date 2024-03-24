using System.Text.Json.Serialization;

namespace FootballAcademy_BackEnd.Entities
{
    public class Staff
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? OtherNames { get; set; }
        public Guid StaffSpecializationId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string? Qualification { get; set; } = String.Empty;
        public string? PassportPicture { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public virtual User User { get; set; }
        public virtual StaffSpecialization StaffSpecialization { get; set; }

    }
}
