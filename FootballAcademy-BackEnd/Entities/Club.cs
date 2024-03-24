namespace FootballAcademy_BackEnd.Entities
{
    public class Club
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Name { get; set; } = String.Empty;
        public string? Contact { get; set; }
        public string? Location { get; set; }
    }
}
