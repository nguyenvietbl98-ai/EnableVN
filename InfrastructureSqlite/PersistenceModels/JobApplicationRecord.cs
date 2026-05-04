namespace InfrastructureSqlite.PersistenceModels;

public sealed class JobApplicationRecord
{
    public Guid Id { get; set; }

    public Guid JobId { get; set; }

    public Guid CandidateId { get; set; }

    public string? CoverLetter { get; set; }

    public string? CvUrl { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; }
}
