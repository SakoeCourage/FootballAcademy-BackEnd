using System.Text.Json.Serialization;

namespace FootballAcademy_BackEnd.Entities
{
    public class PlayerHasTacticalSkills
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public double? UnderstandingOfPositionOfPlay { get; set; }
        public double? FieldDecisionMaking { get; set; }
        public double? Awareness { get; set; }
        public double? TacticalDiscipline { get; set; }
        public double? PassingAccuracy { get; set; }
        public double? AbiityToReadGame { get; set; }
        [JsonIgnore]
        public virtual Player Player { get; set; }

    }
}
