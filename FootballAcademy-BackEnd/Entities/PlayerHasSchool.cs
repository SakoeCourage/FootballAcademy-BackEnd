using System.Text.Json.Serialization;

namespace FootballAcademy_BackEnd.Entities
{
    public class PlayerHasSchool
    {
        public Guid PlayerId { get; set; }
        public Guid SchoolId { get; set; }
        public string? CurrentClass { get; set; }
        [JsonIgnore]
        public virtual Player Player { get; set; }
        public virtual School School { get; set; }

    }
}
