using Domain.Interviews;
using InfrastructureSqlite.PersistenceModels;

namespace InfrastructureSqlite.Mappers;

public static class InterviewSchedulePersistenceMapper
{
    public static InterviewScheduleRecord ToRecord(InterviewSchedule schedule) => new()
    {
        Id = schedule.Id,
        JobApplicationId = schedule.JobApplicationId,
        EmployerUserId = schedule.EmployerUserId,
        CandidateUserId = schedule.CandidateUserId,
        ScheduledAt = schedule.ScheduledAt,
        DurationMinutes = schedule.DurationMinutes,
        InterviewType = schedule.InterviewType.ToString(),
        MeetingLink = schedule.MeetingLink,
        Location = schedule.Location,
        Note = schedule.Note,
        Status = schedule.Status.ToString(),
        CreatedAt = schedule.CreatedAt,
        CandidateRespondedAt = schedule.CandidateRespondedAt,
        CandidateDeclineReason = schedule.CandidateDeclineReason
    };

    public static InterviewSchedule ToDomain(InterviewScheduleRecord r) => InterviewSchedule.Restore(
        id: r.Id,
        jobApplicationId: r.JobApplicationId,
        employerUserId: r.EmployerUserId,
        candidateUserId: r.CandidateUserId,
        scheduledAt: r.ScheduledAt,
        durationMinutes: r.DurationMinutes,
        interviewType: Enum.Parse<InterviewType>(r.InterviewType),
        meetingLink: r.MeetingLink,
        location: r.Location,
        note: r.Note,
        status: Enum.Parse<InterviewStatus>(r.Status),
        createdAt: r.CreatedAt,
        candidateRespondedAt: r.CandidateRespondedAt,
        candidateDeclineReason: r.CandidateDeclineReason);

    public static void UpdateRecord(InterviewScheduleRecord record, InterviewSchedule schedule)
    {
        record.Status = schedule.Status.ToString();
        record.CandidateRespondedAt = schedule.CandidateRespondedAt;
        record.CandidateDeclineReason = schedule.CandidateDeclineReason;
        record.Note = schedule.Note;
    }
}
