using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace FootballAcademy_BackEnd.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Permission
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public virtual ICollection<Role> Roles { get; set; }

    }
}
