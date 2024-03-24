
using System.Text.Json.Serialization;

namespace FootballAcademy_BackEnd.Entities
{
    public class PlayerHasMentalAttribute
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public double? AttituteTowardsSport { get; set; }
        public double? Coachability { get; set; }
        public double? Confidence { get; set; }
        public double? MentalToughness { get; set; }
        public double? FocusAndConcentration { get; set; }
        public double? LeadershipQualities { get; set; }
        [JsonIgnore]
        public virtual Player Player { get; set; }

    }
}
