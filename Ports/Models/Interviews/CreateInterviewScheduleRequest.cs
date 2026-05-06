using Domain.Interviews;

namespace Ports.Models.Interviews;

public sealed class CreateInterviewScheduleRequest
{
    public Guid JobApplicationId { get; init; }
    public DateTime ScheduledAt { get; init; }
    public int DurationMinutes { get; init; } = 60;
    public InterviewType InterviewType { get; init; } = InterviewType.Online;
    public string? MeetingLink { get; init; }
    public string? Location { get; init; }
    public string? Note { get; init; }
}
