using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace FootballAcademy_BackEnd.Entities
{

    // Congenital Deformities
    // Structural abnormalities in bones or joints(e.g., clubfoot, cleft lip or palate, spina bifida).
    //Developmental abnormalities in organs or tissues(e.g., congenital heart defects, neural tube defects).
    //Functional abnormalities affecting movement or coordination(e.g., cerebral palsy).
    //Genetic disorders that affect physical development(e.g., Down syndrome, Turner syndrome).
    [Index(nameof(Email), IsUnique = true)]
    public class Player
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid Id { get; set; }
        public string? PassportPicture { get; set; }
        public string SurName { get; set; } = String.Empty;
        public string FirstName { get; set; } = String.Empty;
        public string? Gender { get; set; }
        public string? OtherNames { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ResidentialAddress { get; set; }
        public string? Community { get; set; }
        public string? ContactNumber { get; set; }
        public string? Nationality { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Socials { get; set; }
        public string GuardianFullName { get; set; } = String.Empty;
        public string? GuardianEmail { get; set; } = String.Empty;
        public string GuardianContactNumber { get; set; } = String.Empty;
        public Boolean AnyCongenitalDeformity { get; set; } = false;
        public string? CongenitalDeformityType { get; set; }
        public string? BloodGroup { get; set; }
        public string? Allergy { get; set; }
        public virtual ICollection<School> School { get; set; }
        public virtual ICollection<PlayerHasMentalAttribute> MentalAttributesHistory { get; set; }
        public virtual ICollection<PlayerHasPhysicalAttribute> PhyiscalAttributesHistory { get; set; }
        public virtual ICollection<PlayerHasTacticalSkills> TacticalSkillsHistory { get; set; }
        public virtual ICollection<PlayerHasTechnicalSkills> TechnicalSiillsHistory { get; set; }
        public virtual ICollection<TrainingGroup> TrainingGroup { get; set; }


    }
}
