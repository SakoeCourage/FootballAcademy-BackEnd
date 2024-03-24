namespace FootballAcademy_BackEnd.Entities
{
    public class Community
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public string CommunityName { get; set; } = string.Empty;
        public string? District { get; set; }

    }
}
