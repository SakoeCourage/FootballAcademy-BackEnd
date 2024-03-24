using System.Text.Json.Serialization;

namespace FootballAcademy_BackEnd.Entities
{
    public class School
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Name { get; set; } = String.Empty;
        public string? PhoneNumber { get; set; } = String.Empty;
        public string? Location { get; set; } = String.Empty;
        [JsonIgnore]
        public virtual ICollection<Player> Players { get; set; }

    }
}
