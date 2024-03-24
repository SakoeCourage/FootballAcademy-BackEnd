


namespace FootballAcademy_BackEnd.Entities
{
    public class Fee
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public string FeeName { get; set; } = string.Empty;
        public string FeeDescription { get; set; } = string.Empty;
        public Double Amount { get; set; }
        public Boolean IsActive { get; set; } = true;

    }
}
