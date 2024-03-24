namespace FootballAcademy_BackEnd.Entities
{
    public class PlayerHasTrainingGroup
    {
        public Guid PlayerId { get; set; }
        public Guid TrainingGroupId { get; set; }
        public virtual TrainingGroup TrainingGroup { get; set; }
        public virtual Player Player { get; set; }

    }
}
