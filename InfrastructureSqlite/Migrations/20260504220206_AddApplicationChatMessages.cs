using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureSqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationChatMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SenderUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Body = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    ModerationOutcome = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ModerationReasonVi = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SentAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationChatMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationChatMessages_JobApplicationId",
                table: "ApplicationChatMessages",
                column: "JobApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationChatMessages");
        }
    }
}
