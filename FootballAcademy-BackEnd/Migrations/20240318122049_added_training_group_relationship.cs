using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballAcademy_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class added_training_group_relationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerHasTrainingGroup",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingGroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerHasTrainingGroup", x => new { x.PlayerId, x.TrainingGroupId });
                    table.ForeignKey(
                        name: "FK_PlayerHasTrainingGroup_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerHasTrainingGroup_TrainingGroup_TrainingGroupId",
                        column: x => x.TrainingGroupId,
                        principalTable: "TrainingGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingGroupHasCoach",
                columns: table => new
                {
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingGroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingGroupHasCoach", x => new { x.CoachId, x.TrainingGroupId });
                    table.ForeignKey(
                        name: "FK_TrainingGroupHasCoach_TrainingGroup_TrainingGroupId",
                        column: x => x.TrainingGroupId,
                        principalTable: "TrainingGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingGroupHasCoach_User_CoachId",
                        column: x => x.CoachId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHasTrainingGroup_TrainingGroupId",
                table: "PlayerHasTrainingGroup",
                column: "TrainingGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingGroupHasCoach_TrainingGroupId",
                table: "TrainingGroupHasCoach",
                column: "TrainingGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerHasTrainingGroup");

            migrationBuilder.DropTable(
                name: "TrainingGroupHasCoach");
        }
    }
}
