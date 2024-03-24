using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballAcademy_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Community",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommunityName = table.Column<string>(type: "text", nullable: false),
                    District = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Community", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PassportPicture = table.Column<string>(type: "text", nullable: true),
                    SurName = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    OtherNames = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResidentialAddress = table.Column<string>(type: "text", nullable: true),
                    Community = table.Column<string>(type: "text", nullable: true),
                    ContactNumber = table.Column<string>(type: "text", nullable: true),
                    Nationality = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Socials = table.Column<string>(type: "text", nullable: true),
                    GuardianFullName = table.Column<string>(type: "text", nullable: false),
                    GuardianEmail = table.Column<string>(type: "text", nullable: true),
                    GuardianContactNumber = table.Column<string>(type: "text", nullable: false),
                    AnyCongenitalDeformity = table.Column<bool>(type: "boolean", nullable: false),
                    CongenitalDeformityType = table.Column<string>(type: "text", nullable: true),
                    BloodGroup = table.Column<string>(type: "text", nullable: true),
                    Allergy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "School",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_School", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaffSpecialization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffSpecialization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerHasMentalAttribute",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AttituteTowardsSport = table.Column<double>(type: "double precision", nullable: true),
                    Coachability = table.Column<double>(type: "double precision", nullable: true),
                    Confidence = table.Column<double>(type: "double precision", nullable: true),
                    MentalToughness = table.Column<double>(type: "double precision", nullable: true),
                    FocusAndConcentration = table.Column<double>(type: "double precision", nullable: true),
                    LeadershipQualities = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerHasMentalAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerHasMentalAttribute_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerHasPhysicalAttribute",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Height = table.Column<double>(type: "double precision", nullable: true),
                    Weight = table.Column<double>(type: "double precision", nullable: true),
                    DominantFoot = table.Column<string>(type: "text", nullable: false),
                    Speed = table.Column<double>(type: "double precision", nullable: true),
                    Endurance = table.Column<double>(type: "double precision", nullable: true),
                    Strength = table.Column<double>(type: "double precision", nullable: true),
                    Agility = table.Column<double>(type: "double precision", nullable: true),
                    Flexibility = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerHasPhysicalAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerHasPhysicalAttribute_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerHasTacticalSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UnderstandingOfPositionOfPlay = table.Column<double>(type: "double precision", nullable: true),
                    FieldDecisionMaking = table.Column<double>(type: "double precision", nullable: true),
                    Awareness = table.Column<double>(type: "double precision", nullable: true),
                    TacticalDiscipline = table.Column<double>(type: "double precision", nullable: true),
                    PassingAccuracy = table.Column<double>(type: "double precision", nullable: true),
                    AbiityToReadGame = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerHasTacticalSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerHasTacticalSkills_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerHasTechnicalSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PassingAccuracy = table.Column<double>(type: "double precision", nullable: true),
                    ShootingAccuracy = table.Column<double>(type: "double precision", nullable: true),
                    DribblingAbility = table.Column<double>(type: "double precision", nullable: true),
                    BallControll = table.Column<double>(type: "double precision", nullable: true),
                    HeadingAbility = table.Column<double>(type: "double precision", nullable: true),
                    TacklingAbility = table.Column<double>(type: "double precision", nullable: true),
                    CrossingAbility = table.Column<double>(type: "double precision", nullable: true),
                    SetPieceProficiency = table.Column<double>(type: "double precision", nullable: true),
                    WeakFootProficiency = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerHasTechnicalSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerHasTechnicalSkills_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleHasPermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleHasPermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RoleHasPermissions_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleHasPermissions_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    EmailVerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerHasSchool",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentClass = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerHasSchool", x => new { x.PlayerId, x.SchoolId });
                    table.ForeignKey(
                        name: "FK_PlayerHasSchool_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerHasSchool_School_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "School",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    OtherNames = table.Column<string>(type: "text", nullable: true),
                    StaffSpecializationId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Qualification = table.Column<string>(type: "text", nullable: true),
                    PassportPicture = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Staff_StaffSpecialization_StaffSpecializationId",
                        column: x => x.StaffSpecializationId,
                        principalTable: "StaffSpecialization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Staff_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserHasRole",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHasRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserHasRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserHasRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Name",
                table: "Permission",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Player_Email",
                table: "Player",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHasMentalAttribute_PlayerId",
                table: "PlayerHasMentalAttribute",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHasPhysicalAttribute_PlayerId",
                table: "PlayerHasPhysicalAttribute",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHasSchool_SchoolId",
                table: "PlayerHasSchool",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHasTacticalSkills_PlayerId",
                table: "PlayerHasTacticalSkills",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHasTechnicalSkills_PlayerId",
                table: "PlayerHasTechnicalSkills",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Name",
                table: "Role",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleHasPermissions_PermissionId",
                table: "RoleHasPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_StaffSpecializationId",
                table: "Staff",
                column: "StaffSpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_UserId",
                table: "Staff",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaffSpecialization_Name",
                table: "StaffSpecialization",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHasRole_RoleId",
                table: "UserHasRole",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Community");

            migrationBuilder.DropTable(
                name: "PlayerHasMentalAttribute");

            migrationBuilder.DropTable(
                name: "PlayerHasPhysicalAttribute");

            migrationBuilder.DropTable(
                name: "PlayerHasSchool");

            migrationBuilder.DropTable(
                name: "PlayerHasTacticalSkills");

            migrationBuilder.DropTable(
                name: "PlayerHasTechnicalSkills");

            migrationBuilder.DropTable(
                name: "RoleHasPermissions");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "UserHasRole");

            migrationBuilder.DropTable(
                name: "School");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "StaffSpecialization");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
