namespace FootballAcademy_BackEnd.Entities
{
    public class UserHasRole
    {

        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
