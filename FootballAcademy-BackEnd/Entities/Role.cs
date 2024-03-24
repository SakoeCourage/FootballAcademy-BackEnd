using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace FootballAcademy_BackEnd.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Permission> Permissions { get; set; }

        [JsonIgnore]
        public virtual ICollection<User> Users { get; set; }
    }
}
