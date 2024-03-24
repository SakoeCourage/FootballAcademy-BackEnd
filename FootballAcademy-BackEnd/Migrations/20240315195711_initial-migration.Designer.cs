﻿// <auto-generated />
using System;
using FootballAcademy_BackEnd.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FootballAcademy_BackEnd.Migrations
{
    [DbContext(typeof(FootballAcademyDBContext))]
    [Migration("20240315195711_initial-migration")]
    partial class initialmigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.Community", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CommunityName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("District")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Community");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.Permission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Permission");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Allergy")
                        .HasColumnType("text");

                    b.Property<bool>("AnyCongenitalDeformity")
                        .HasColumnType("boolean");

                    b.Property<string>("BloodGroup")
                        .HasColumnType("text");

                    b.Property<string>("Community")
                        .HasColumnType("text");

                    b.Property<string>("CongenitalDeformityType")
                        .HasColumnType("text");

                    b.Property<string>("ContactNumber")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Gender")
                        .HasColumnType("text");

                    b.Property<string>("GuardianContactNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("GuardianEmail")
                        .HasColumnType("text");

                    b.Property<string>("GuardianFullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Nationality")
                        .HasColumnType("text");

                    b.Property<string>("OtherNames")
                        .HasColumnType("text");

                    b.Property<string>("PassportPicture")
                        .HasColumnType("text");

                    b.Property<string>("ResidentialAddress")
                        .HasColumnType("text");

                    b.Property<string>("Socials")
                        .HasColumnType("text");

                    b.Property<string>("SurName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Player");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.PlayerHasMentalAttribute", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double?>("AttituteTowardsSport")
                        .HasColumnType("double precision");

                    b.Property<double?>("Coachability")
                        .HasColumnType("double precision");

                    b.Property<double?>("Confidence")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double?>("FocusAndConcentration")
                        .HasColumnType("double precision");

                    b.Property<double?>("LeadershipQualities")
                        .HasColumnType("double precision");

                    b.Property<double?>("MentalToughness")
                        .HasColumnType("double precision");

                    b.Property<Guid>("PlayerId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerHasMentalAttribute");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.PlayerHasPhysicalAttribute", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double?>("Agility")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DominantFoot")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double?>("Endurance")
                        .HasColumnType("double precision");

                    b.Property<double?>("Flexibility")
                        .HasColumnType("double precision");

                    b.Property<double?>("Height")
                        .HasColumnType("double precision");

                    b.Property<Guid>("PlayerId")
                        .HasColumnType("uuid");

                    b.Property<double?>("Speed")
                        .HasColumnType("double precision");

                    b.Property<double?>("Strength")
                        .HasColumnType("double precision");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double?>("Weight")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerHasPhysicalAttribute");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.PlayerHasSchool", b =>
                {
                    b.Property<Guid>("PlayerId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SchoolId")
                        .HasColumnType("uuid");

                    b.Property<string>("CurrentClass")
                        .HasColumnType("text");

                    b.HasKey("PlayerId", "SchoolId");

                    b.HasIndex("SchoolId");

                    b.ToTable("PlayerHasSchool", (string)null);
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.PlayerHasTacticalSkills", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double?>("AbiityToReadGame")
                        .HasColumnType("double precision");

                    b.Property<double?>("Awareness")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double?>("FieldDecisionMaking")
                        .HasColumnType("double precision");

                    b.Property<double?>("PassingAccuracy")
                        .HasColumnType("double precision");

                    b.Property<Guid>("PlayerId")
                        .HasColumnType("uuid");

                    b.Property<double?>("TacticalDiscipline")
                        .HasColumnType("double precision");

                    b.Property<double?>("UnderstandingOfPositionOfPlay")
                        .HasColumnType("double precision");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerHasTacticalSkills");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.PlayerHasTechnicalSkills", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double?>("BallControll")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double?>("CrossingAbility")
                        .HasColumnType("double precision");

                    b.Property<double?>("DribblingAbility")
                        .HasColumnType("double precision");

                    b.Property<double?>("HeadingAbility")
                        .HasColumnType("double precision");

                    b.Property<double?>("PassingAccuracy")
                        .HasColumnType("double precision");

                    b.Property<Guid>("PlayerId")
                        .HasColumnType("uuid");

                    b.Property<double?>("SetPieceProficiency")
                        .HasColumnType("double precision");

                    b.Property<double?>("ShootingAccuracy")
                        .HasColumnType("double precision");

                    b.Property<double?>("TacklingAbility")
                        .HasColumnType("double precision");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double?>("WeakFootProficiency")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerHasTechnicalSkills");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Role");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.RoleHasPermissions", b =>
                {
                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PermissionId")
                        .HasColumnType("uuid");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("RoleHasPermissions", (string)null);
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.School", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Location")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("School");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.Staff", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("OtherNames")
                        .HasColumnType("text");

                    b.Property<string>("PassportPicture")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Qualification")
                        .HasColumnType("text");

                    b.Property<Guid>("StaffSpecializationId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("StaffSpecializationId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Staff");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.StaffSpecialization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("StaffSpecialization");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("EmailVerifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.UserHasRole", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserHasRole");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.PlayerHasMentalAttribute", b =>
                {
                    b.HasOne("FootballAcademy_BackEnd.Entities.Player", "Player")
                        .WithMany("MentalAttributesHistory")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.PlayerHasPhysicalAttribute", b =>
                {
                    b.HasOne("FootballAcademy_BackEnd.Entities.Player", "Player")
                        .WithMany("PhyiscalAttributesHistory")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.PlayerHasSchool", b =>
                {
                    b.HasOne("FootballAcademy_BackEnd.Entities.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FootballAcademy_BackEnd.Entities.School", "School")
                        .WithMany()
                        .HasForeignKey("SchoolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("School");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.PlayerHasTacticalSkills", b =>
                {
                    b.HasOne("FootballAcademy_BackEnd.Entities.Player", "Player")
                        .WithMany("TacticalSkillsHistory")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.PlayerHasTechnicalSkills", b =>
                {
                    b.HasOne("FootballAcademy_BackEnd.Entities.Player", "Player")
                        .WithMany("TechnicalSiillsHistory")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.RoleHasPermissions", b =>
                {
                    b.HasOne("FootballAcademy_BackEnd.Entities.Permission", "Permission")
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FootballAcademy_BackEnd.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.Staff", b =>
                {
                    b.HasOne("FootballAcademy_BackEnd.Entities.StaffSpecialization", "StaffSpecialization")
                        .WithMany()
                        .HasForeignKey("StaffSpecializationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FootballAcademy_BackEnd.Entities.User", "User")
                        .WithOne("Staff")
                        .HasForeignKey("FootballAcademy_BackEnd.Entities.Staff", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StaffSpecialization");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.User", b =>
                {
                    b.HasOne("FootballAcademy_BackEnd.Entities.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.UserHasRole", b =>
                {
                    b.HasOne("FootballAcademy_BackEnd.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FootballAcademy_BackEnd.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.Player", b =>
                {
                    b.Navigation("MentalAttributesHistory");

                    b.Navigation("PhyiscalAttributesHistory");

                    b.Navigation("TacticalSkillsHistory");

                    b.Navigation("TechnicalSiillsHistory");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("FootballAcademy_BackEnd.Entities.User", b =>
                {
                    b.Navigation("Staff")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}