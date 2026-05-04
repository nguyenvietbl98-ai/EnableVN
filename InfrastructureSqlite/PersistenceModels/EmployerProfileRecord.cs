namespace InfrastructureSqlite.PersistenceModels;

public sealed class EmployerProfileRecord
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? WebsiteUrl { get; set; }

    public bool HasWheelchairAccess { get; set; }

    public bool HasAccessibleRestroom { get; set; }

    public bool SupportsFlexibleWorkingTime { get; set; }

    public bool SupportsRemoteWork { get; set; }

    public bool ProvidesAssistiveDevices { get; set; }
}
