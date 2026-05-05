namespace InfrastructureSqlite.PersistenceModels;

public sealed class EmployerProfileRecord
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }

    public string? ContactEmail { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? CompanySize { get; set; }

    public string? Industry { get; set; }

    public string? TaxCode { get; set; }

    public string? RecruiterContactName { get; set; }

    public string? RecruiterContactTitle { get; set; }

    public string? Description { get; set; }

    public string? Benefits { get; set; }

    public string? Culture { get; set; }

    public string? WebsiteUrl { get; set; }

    public string VerificationStatus { get; set; } = string.Empty;

    public DateTime? VerifiedAtUtc { get; set; }

    public string? VerificationNote { get; set; }

    public bool HasWheelchairAccess { get; set; }

    public bool HasAccessibleRestroom { get; set; }

    public bool SupportsFlexibleWorkingTime { get; set; }

    public bool SupportsRemoteWork { get; set; }

    public bool ProvidesAssistiveDevices { get; set; }
}
