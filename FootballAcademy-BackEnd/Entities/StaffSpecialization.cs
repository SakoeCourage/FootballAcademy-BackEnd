using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class StaffSpecialization
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Name { get; set; }
    }
}
