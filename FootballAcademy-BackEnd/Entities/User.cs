using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FootballAcademy_BackEnd.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(Role))]
        public Guid RoleId { get; set; }
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public string Password { get; set; }
        public virtual Staff Staff { get; set; }
        [JsonIgnore]
        public virtual Role Role { get; set; }
        public virtual ICollection<TrainingGroup> TrainingGroup { get; set; }
    }
}
