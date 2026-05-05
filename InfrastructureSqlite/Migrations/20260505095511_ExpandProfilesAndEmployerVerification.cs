using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureSqlite.Migrations
{
    /// <inheritdoc />
    public partial class ExpandProfilesAndEmployerVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "EmployerProfiles",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Benefits",
                table: "EmployerProfiles",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanySize",
                table: "EmployerProfiles",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "EmployerProfiles",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Culture",
                table: "EmployerProfiles",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "EmployerProfiles",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "EmployerProfiles",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "EmployerProfiles",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecruiterContactName",
                table: "EmployerProfiles",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecruiterContactTitle",
                table: "EmployerProfiles",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxCode",
                table: "EmployerProfiles",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationNote",
                table: "EmployerProfiles",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "EmployerProfiles",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAtUtc",
                table: "EmployerProfiles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccessibilityNeeds",
                table: "CandidateProfiles",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "CandidateProfiles",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "CandidateProfiles",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Certifications",
                table: "CandidateProfiles",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "CandidateProfiles",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "CandidateProfiles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DesiredPosition",
                table: "CandidateProfiles",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DesiredSalary",
                table: "CandidateProfiles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DesiredWorkMode",
                table: "CandidateProfiles",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "CandidateProfiles",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExperienceSummary",
                table: "CandidateProfiles",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "CandidateProfiles",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobSeekingStatus",
                table: "CandidateProfiles",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "CandidateProfiles",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PortfolioUrl",
                table: "CandidateProfiles",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "CandidateProfiles",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "Benefits",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "CompanySize",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "Culture",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "RecruiterContactName",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "RecruiterContactTitle",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "TaxCode",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "VerificationNote",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "VerifiedAtUtc",
                table: "EmployerProfiles");

            migrationBuilder.DropColumn(
                name: "AccessibilityNeeds",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "Certifications",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "DesiredPosition",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "DesiredSalary",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "DesiredWorkMode",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "ExperienceSummary",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "JobSeekingStatus",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "PortfolioUrl",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "CandidateProfiles");
        }
    }
}
