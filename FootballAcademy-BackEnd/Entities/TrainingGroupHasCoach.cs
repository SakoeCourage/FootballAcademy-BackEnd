namespace FootballAcademy_BackEnd.Entities
{
    public class TrainingGroupHasCoach
    {
        public Guid CoachId { get; set; }
        public Guid TrainingGroupId { get; set; }
        public virtual TrainingGroup TrainingGroup { get; set; }
        public virtual User Coach { get; set; }

    }
}
