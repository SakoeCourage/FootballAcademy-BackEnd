using FootballAcademy_BackEnd.Entities;
using Microsoft.EntityFrameworkCore;

namespace FootballAcademy_BackEnd.Database
{
    public class FootballAcademyDBContext : DbContext
    {
        public FootballAcademyDBContext(DbContextOptions<FootballAcademyDBContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<StaffSpecialization> StaffSpecialization { get; set; }
        public DbSet<UserHasRole> UserHasRole { get; set; }
        public DbSet<RoleHasPermissions> RoleHasPermissions { get; set; }
        public DbSet<Player> Player { get; set; }
        public DbSet<PlayerHasMentalAttribute> PlayerHasMentalAttribute { get; set; }
        public DbSet<PlayerHasPhysicalAttribute> PlayerHasPhysicalAttribute { get; set; }
        public DbSet<PlayerHasTacticalSkills> PlayerHasTacticalSkills { get; set; }
        public DbSet<PlayerHasTechnicalSkills> PlayerHasTechnicalSkills { get; set; }
        public DbSet<PlayerHasSchool> PlayerHasSchool { get; set; }
        public DbSet<School> School { get; set; }
        public DbSet<Community> Community { get; set; }
        public DbSet<Club> Club { get; set; }
        public DbSet<TrainingGroup> TrainingGroup { get; set; }
        public DbSet<Fee> Fee { get; set; }
        public DbSet<PlayerHasTrainingGroup> PlayerHasTrainingGroup { get; set; }
        public DbSet<TrainingGroupHasCoach> TrainingGroupHasCoach { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Staff>()
              .HasOne(e => e.StaffSpecialization)
              .WithMany()
              .HasForeignKey(e => e.StaffSpecializationId);

            modelBuilder.Entity<Staff>()
                .HasOne(s => s.User)
                .WithOne(u => u.Staff)
                .HasForeignKey<Staff>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(s => s.Role)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.RoleId)
                .HasPrincipalKey(u => u.Id)
                ;

            modelBuilder.Entity<UserHasRole>()
               .HasKey(sr => new { sr.UserId, sr.RoleId });

            modelBuilder.Entity<Role>()
                    .HasMany(r => r.Permissions)
                    .WithMany(p => p.Roles)
                    .UsingEntity<RoleHasPermissions>(
                        j => j
                            .HasOne(rp => rp.Permission)
                            .WithMany()
                            .HasForeignKey(rp => rp.PermissionId)
                            .OnDelete(DeleteBehavior.Cascade)
                            ,
                        j => j
                            .HasOne(rp => rp.Role)
                            .WithMany()
                            .HasForeignKey(rp => rp.RoleId)
                            .OnDelete(DeleteBehavior.Cascade)
                            ,
                        j =>
                        {
                            j.HasKey(rp => new { rp.RoleId, rp.PermissionId });
                            j.ToTable("RoleHasPermissions");
                        });





            modelBuilder.Entity<Player>()
                        .HasMany(p => p.MentalAttributesHistory)
                        .WithOne(at => at.Player)
                        .HasForeignKey(at => at.PlayerId)
                        .OnDelete(DeleteBehavior.Cascade)
                        ;

            modelBuilder.Entity<Player>()
                      .HasMany(p => p.PhyiscalAttributesHistory)
                      .WithOne(at => at.Player)
                      .HasForeignKey(at => at.PlayerId)
                      .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Player>()
                     .HasMany(p => p.TechnicalSiillsHistory)
                     .WithOne(at => at.Player)
                     .HasForeignKey(at => at.PlayerId)
                     .OnDelete(DeleteBehavior.Cascade)
                     ;

            modelBuilder.Entity<Player>()
                     .HasMany(p => p.TacticalSkillsHistory)
                     .WithOne(at => at.Player)
                     .HasForeignKey(at => at.PlayerId)
                     .OnDelete(DeleteBehavior.Cascade)
                     ;

            modelBuilder.Entity<Player>()
            .HasMany(p => p.School)
            .WithMany(s => s.Players)
            .UsingEntity<PlayerHasSchool>(
                j => j
                    .HasOne(ps => ps.School)
                    .WithMany()
                    .HasForeignKey(p => p.SchoolId)
                    .OnDelete(DeleteBehavior.Cascade)
                    ,
                j => j
                    .HasOne(ps => ps.Player)
                    .WithMany()
                    .HasForeignKey(p => p.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey(ps => new { ps.PlayerId, ps.SchoolId });
                    j.ToTable("PlayerHasSchool");
                }
            );

            modelBuilder.Entity<Player>()
                .HasMany(p => p.TrainingGroup)
                .WithMany(tg => tg.Players)
                .UsingEntity<PlayerHasTrainingGroup>(
                    j => j
                    .HasOne(tg => tg.TrainingGroup)
                    .WithMany()
                    .HasForeignKey(f => f.TrainingGroupId)
                    .OnDelete(DeleteBehavior.Cascade),
                    j => j
                    .HasOne(f => f.Player)
                    .WithMany()
                    .HasForeignKey(p => p.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade),
                    j =>
                   {
                       j.HasKey(ps => new { ps.PlayerId, ps.TrainingGroupId });
                       j.ToTable("PlayerHasTrainingGroup");
                   }

                );

            modelBuilder.Entity<User>()
           .HasMany(p => p.TrainingGroup)
           .WithMany(tg => tg.Coach)
           .UsingEntity<TrainingGroupHasCoach>(
               j => j
               .HasOne(tg => tg.TrainingGroup)
               .WithMany()
               .HasForeignKey(f => f.TrainingGroupId)
               .OnDelete(DeleteBehavior.Cascade),
               j => j
               .HasOne(f => f.Coach)
               .WithMany()
               .HasForeignKey(p => p.CoachId)
               .OnDelete(DeleteBehavior.Cascade),
               j =>
               {
                   j.HasKey(ps => new { ps.CoachId, ps.TrainingGroupId });
                   j.ToTable("TrainingGroupHasCoach");
               }

           );

        }




    }
}
