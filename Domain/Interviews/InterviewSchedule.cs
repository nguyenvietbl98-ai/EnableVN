using Domain.Common;

namespace Domain.Interviews;

/// <summary>
/// Aggregate root cho lịch phỏng vấn.
/// Employer tạo, Candidate xác nhận hoặc từ chối.
/// </summary>
public sealed class InterviewSchedule : AggregateRoot<Guid>
{
    public Guid JobApplicationId { get; private set; }

    /// <summary>UserId của employer (để gửi notification phản hồi)</summary>
    public Guid EmployerUserId { get; private set; }

    /// <summary>UserId của candidate (để check quyền & gửi notification)</summary>
    public Guid CandidateUserId { get; private set; }

    public DateTime ScheduledAt { get; private set; }
    public int DurationMinutes { get; private set; }
    public InterviewType InterviewType { get; private set; }
    public string? MeetingLink { get; private set; }
    public string? Location { get; private set; }
    public string? Note { get; private set; }
    public InterviewStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CandidateRespondedAt { get; private set; }
    public string? CandidateDeclineReason { get; private set; }

    private InterviewSchedule(
        Guid id,
        Guid jobApplicationId,
        Guid employerUserId,
        Guid candidateUserId,
        DateTime scheduledAt,
        int durationMinutes,
        InterviewType interviewType,
        string? meetingLink,
        string? location,
        string? note
    ) : base(id)
    {
        JobApplicationId = jobApplicationId;
        EmployerUserId = employerUserId;
        CandidateUserId = candidateUserId;
        ScheduledAt = scheduledAt;
        DurationMinutes = durationMinutes;
        InterviewType = interviewType;
        MeetingLink = meetingLink;
        Location = location;
        Note = note;
        Status = InterviewStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public static InterviewSchedule Create(
        Guid jobApplicationId,
        Guid employerUserId,
        Guid candidateUserId,
        DateTime scheduledAt,
        int durationMinutes,
        InterviewType interviewType,
        string? meetingLink,
        string? location,
        string? note)
    {
        if (jobApplicationId == Guid.Empty)
            throw new DomainException("JobApplicationId không hợp lệ.");

        if (employerUserId == Guid.Empty)
            throw new DomainException("EmployerUserId không hợp lệ.");

        if (candidateUserId == Guid.Empty)
            throw new DomainException("CandidateUserId không hợp lệ.");

        if (scheduledAt <= DateTime.UtcNow)
            throw new DomainException("Thời gian phỏng vấn phải lớn hơn thời gian hiện tại.");

        if (durationMinutes < 15)
            throw new DomainException("Thời lượng phỏng vấn tối thiểu 15 phút.");

        if (interviewType == InterviewType.Online && string.IsNullOrWhiteSpace(meetingLink))
            throw new DomainException("Phỏng vấn Online bắt buộc phải có Meeting Link.");

        if (interviewType == InterviewType.Offline && string.IsNullOrWhiteSpace(location))
            throw new DomainException("Phỏng vấn Offline bắt buộc phải có địa điểm.");

        return new InterviewSchedule(
            Guid.NewGuid(),
            jobApplicationId,
            employerUserId,
            candidateUserId,
            scheduledAt,
            durationMinutes,
            interviewType,
            meetingLink?.Trim(),
            location?.Trim(),
            note?.Trim());
    }

    public void Accept()
    {
        if (Status != InterviewStatus.Pending)
            throw new DomainException("Chỉ có thể xác nhận lịch đang ở trạng thái Chờ phản hồi.");

        Status = InterviewStatus.Accepted;
        CandidateRespondedAt = DateTime.UtcNow;
    }

    public void Decline(string? reason)
    {
        if (Status != InterviewStatus.Pending)
            throw new DomainException("Chỉ có thể từ chối lịch đang ở trạng thái Chờ phản hồi.");

        Status = InterviewStatus.Declined;
        CandidateRespondedAt = DateTime.UtcNow;
        CandidateDeclineReason = reason?.Trim();
    }

    public void Cancel(string? reason)
    {
        if (Status == InterviewStatus.Completed)
            throw new DomainException("Không thể hủy lịch đã hoàn thành.");

        if (Status == InterviewStatus.Cancelled)
            throw new DomainException("Lịch đã được hủy trước đó.");

        Status = InterviewStatus.Cancelled;
        Note = string.IsNullOrWhiteSpace(reason) ? Note : $"[Hủy] {reason?.Trim()}";
    }

    public static InterviewSchedule Restore(
        Guid id,
        Guid jobApplicationId,
        Guid employerUserId,
        Guid candidateUserId,
        DateTime scheduledAt,
        int durationMinutes,
        InterviewType interviewType,
        string? meetingLink,
        string? location,
        string? note,
        InterviewStatus status,
        DateTime createdAt,
        DateTime? candidateRespondedAt,
        string? candidateDeclineReason)
    {
        var schedule = new InterviewSchedule(
            id,
            jobApplicationId,
            employerUserId,
            candidateUserId,
            scheduledAt,
            durationMinutes,
            interviewType,
            meetingLink,
            location,
            note);

        schedule.Status = status;
        schedule.CreatedAt = createdAt;
        schedule.CandidateRespondedAt = candidateRespondedAt;
        schedule.CandidateDeclineReason = candidateDeclineReason;
        return schedule;
    }
}
