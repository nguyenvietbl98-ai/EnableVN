namespace InfrastructureSqlite.PersistenceModels;

public sealed class InterviewScheduleRecord
{
    public Guid Id { get; set; }
    public Guid JobApplicationId { get; set; }
    public Guid EmployerUserId { get; set; }
    public Guid CandidateUserId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public string InterviewType { get; set; } = string.Empty;
    public string? MeetingLink { get; set; }
    public string? Location { get; set; }
    public string? Note { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CandidateRespondedAt { get; set; }
    public string? CandidateDeclineReason { get; set; }
}
