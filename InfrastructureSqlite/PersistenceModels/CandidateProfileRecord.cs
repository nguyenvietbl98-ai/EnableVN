namespace InfrastructureSqlite.PersistenceModels;

public sealed class CandidateProfileRecord
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ContactEmail { get; set; }

    public string? Address { get; set; }

    public string? DesiredPosition { get; set; }

    public decimal? DesiredSalary { get; set; }

    public string? ExperienceSummary { get; set; }

    public string? Skills { get; set; }

    public string? Education { get; set; }

    public string? Certifications { get; set; }

    public string? PortfolioUrl { get; set; }

    public string? Bio { get; set; }

    public string? CvUrl { get; set; }

    public string? JobSeekingStatus { get; set; }

    public string? DesiredWorkMode { get; set; }

    public string? AccessibilityNeeds { get; set; }

    public Guid? DisabilityTypeId { get; set; }

    public string? DisabilityDescription { get; set; }

    public bool IsDisabilityVisibleToEmployer { get; set; }

    public bool IsPublicProfile { get; set; }
}
