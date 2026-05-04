namespace InfrastructureSqlite.PersistenceModels;

public sealed class DisabilityTypeRecord
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Status { get; set; } = string.Empty;
}
