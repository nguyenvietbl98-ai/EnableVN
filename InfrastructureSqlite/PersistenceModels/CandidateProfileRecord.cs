namespace InfrastructureSqlite.PersistenceModels;

public sealed class CandidateProfileRecord
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? Bio { get; set; }

    public string? CvUrl { get; set; }

    public Guid? DisabilityTypeId { get; set; }

    public string? DisabilityDescription { get; set; }

    public bool IsDisabilityVisibleToEmployer { get; set; }

    public bool IsPublicProfile { get; set; }
}
