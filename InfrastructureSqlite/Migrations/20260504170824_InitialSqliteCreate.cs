using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureSqlite.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqliteCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationStatusHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssistiveDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssistiveDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CandidateProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CvUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DisabilityTypeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DisabilityDescription = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsDisabilityVisibleToEmployer = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPublicProfile = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DisabilityTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisabilityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployerProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    HasWheelchairAccess = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasAccessibleRestroom = table.Column<bool>(type: "INTEGER", nullable: false),
                    SupportsFlexibleWorkingTime = table.Column<bool>(type: "INTEGER", nullable: false),
                    SupportsRemoteWork = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProvidesAssistiveDevices = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployerProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    JobId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CandidateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CoverLetter = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CvUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EmployerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    Requirement = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    WorkMode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MinSalary = table.Column<decimal>(type: "TEXT", nullable: true),
                    MaxSalary = table.Column<decimal>(type: "TEXT", nullable: true),
                    SupportsWheelchairAccess = table.Column<bool>(type: "INTEGER", nullable: false),
                    SupportsRemoteWork = table.Column<bool>(type: "INTEGER", nullable: false),
                    SupportsFlexibleTime = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProvidesAssistiveDevices = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessibilityAdditionalInfo = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStatusHistories_JobApplicationId",
                table: "ApplicationStatusHistories",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_AssistiveDevices_Status",
                table: "AssistiveDevices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_UserId",
                table: "CandidateProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DisabilityTypes_Status",
                table: "DisabilityTypes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EmployerProfiles_UserId",
                table: "EmployerProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_CandidateId",
                table: "JobApplications",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_JobId",
                table: "JobApplications",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_JobId_CandidateId",
                table: "JobApplications",
                columns: new[] { "JobId", "CandidateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobCategories_Status",
                table: "JobCategories",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_EmployerId",
                table: "JobPosts",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_Status",
                table: "JobPosts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_Status_EmployerId",
                table: "JobPosts",
                columns: new[] { "Status", "EmployerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationStatusHistories");

            migrationBuilder.DropTable(
                name: "AssistiveDevices");

            migrationBuilder.DropTable(
                name: "CandidateProfiles");

            migrationBuilder.DropTable(
                name: "DisabilityTypes");

            migrationBuilder.DropTable(
                name: "EmployerProfiles");

            migrationBuilder.DropTable(
                name: "JobApplications");

            migrationBuilder.DropTable(
                name: "JobCategories");

            migrationBuilder.DropTable(
                name: "JobPosts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
