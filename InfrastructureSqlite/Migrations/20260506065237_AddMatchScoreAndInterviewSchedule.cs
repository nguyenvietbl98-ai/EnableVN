using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureSqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchScoreAndInterviewSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MatchLevel",
                table: "JobApplications",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MatchReason",
                table: "JobApplications",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "MatchScore",
                table: "JobApplications",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "InterviewSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EmployerUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CandidateUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    InterviewType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MeetingLink = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CandidateRespondedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CandidateDeclineReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewSchedules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedules_CandidateUserId",
                table: "InterviewSchedules",
                column: "CandidateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedules_EmployerUserId",
                table: "InterviewSchedules",
                column: "EmployerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedules_JobApplicationId",
                table: "InterviewSchedules",
                column: "JobApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewSchedules");

            migrationBuilder.DropColumn(
                name: "MatchLevel",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "MatchReason",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "MatchScore",
                table: "JobApplications");
        }
    }
}
