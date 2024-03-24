using System.Text.Json.Serialization;

namespace FootballAcademy_BackEnd.Entities
{
    public class PlayerHasTechnicalSkills
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public double? PassingAccuracy { get; set; }
        public double? ShootingAccuracy { get; set; }
        public double? DribblingAbility { get; set; }
        public double? BallControll { get; set; }
        public double? HeadingAbility { get; set; }
        public double? TacklingAbility { get; set; }
        public double? CrossingAbility { get; set; }
        public double? SetPieceProficiency { get; set; }
        public double? WeakFootProficiency { get; set; }
        [JsonIgnore]
        public virtual Player Player { get; set; }

    }
}
