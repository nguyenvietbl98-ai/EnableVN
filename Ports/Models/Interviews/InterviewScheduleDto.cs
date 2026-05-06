using Domain.Interviews;

namespace Ports.Models.Interviews;

public sealed class InterviewScheduleDto
{
    public Guid Id { get; init; }
    public Guid JobApplicationId { get; init; }
    public Guid EmployerUserId { get; init; }
    public Guid CandidateUserId { get; init; }
    public string CandidateName { get; init; } = string.Empty;
    public string EmployerCompanyName { get; init; } = string.Empty;
    public string JobTitle { get; init; } = string.Empty;
    public DateTime ScheduledAt { get; init; }
    public int DurationMinutes { get; init; }
    public string InterviewType { get; init; } = string.Empty;
    public string? MeetingLink { get; init; }
    public string? Location { get; init; }
    public string? Note { get; init; }
    public string Status { get; init; } = string.Empty;
    public string StatusVi { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? CandidateRespondedAt { get; init; }
    public string? CandidateDeclineReason { get; init; }
}
