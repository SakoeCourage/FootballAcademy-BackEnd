
using System.Text.Json.Serialization;

namespace FootballAcademy_BackEnd.Entities
{
    public class PlayerHasPhysicalAttribute
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public string DominantFoot { get; set; } = String.Empty;
        public double? Speed { get; set; }
        public double? Endurance { get; set; }
        public double? Strength { get; set; }
        public double? Agility { get; set; }
        public double? Flexibility { get; set; }

        [JsonIgnore]
        public virtual Player Player { get; set; }


    }
}
