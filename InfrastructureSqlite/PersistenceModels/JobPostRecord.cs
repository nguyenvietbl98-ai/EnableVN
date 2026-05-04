namespace InfrastructureSqlite.PersistenceModels;

public sealed class JobPostRecord
{
    public Guid Id { get; set; }

    public Guid EmployerId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Requirement { get; set; } = string.Empty;

    public string WorkMode { get; set; } = string.Empty;

    public decimal? MinSalary { get; set; }

    public decimal? MaxSalary { get; set; }

    public bool SupportsWheelchairAccess { get; set; }

    public bool SupportsRemoteWork { get; set; }

    public bool SupportsFlexibleTime { get; set; }

    public bool ProvidesAssistiveDevices { get; set; }

    public string? AccessibilityAdditionalInfo { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime? ClosedAt { get; set; }
}
