namespace InfrastructureSqlite.PersistenceModels;

public sealed class ApplicationStatusHistoryRecord
{
    public Guid Id { get; set; }

    public Guid JobApplicationId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Note { get; set; }

    public DateTime ChangedAt { get; set; }
}
