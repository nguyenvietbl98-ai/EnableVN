using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Candidates
{
    /// <summary>
    /// Command cập nhật thông tin cơ bản của ứng viên.
    /// </summary>
    public sealed class UpdateCandidateProfileCommand
    {
        public string FullName { get; init; } = string.Empty;

    public string? AvatarUrl { get; init; }

    public DateTime? DateOfBirth { get; init; }

    public string? Gender { get; init; }

    public string? PhoneNumber { get; init; }

    public string? ContactEmail { get; init; }

    public string? Address { get; init; }

    public string? DesiredPosition { get; init; }

    public decimal? DesiredSalary { get; init; }

    public string? ExperienceSummary { get; init; }

    public string? Skills { get; init; }

    public string? Education { get; init; }

    public string? Certifications { get; init; }

    public string? PortfolioUrl { get; init; }

        public string? Bio { get; init; }

        public string? CvUrl { get; init; }

    public string? JobSeekingStatus { get; init; }

    public string? DesiredWorkMode { get; init; }

    public string? AccessibilityNeeds { get; init; }
    }
}
