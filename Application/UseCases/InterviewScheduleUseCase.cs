using Application.Common;
using Application.Email;
using Domain.Interviews;
using Domain.Notifications;
using Microsoft.Extensions.Logging;
using Ports.Inbound;
using Ports.Models.Interviews;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;

namespace Application.UseCases;

public sealed class InterviewScheduleUseCase : IInterviewScheduleUseCase
{
    private readonly IInterviewScheduleRepository _interviewRepository;
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IJobRepository _jobRepository;
    private readonly ICandidateProfileRepository _candidateProfileRepository;
    private readonly IEmployerProfileRepository _employerProfileRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IEmailService _emailService;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<InterviewScheduleUseCase> _logger;

    public InterviewScheduleUseCase(
        IInterviewScheduleRepository interviewRepository,
        IJobApplicationRepository jobApplicationRepository,
        IJobRepository jobRepository,
        ICandidateProfileRepository candidateProfileRepository,
        IEmployerProfileRepository employerProfileRepository,
        IUserRepository userRepository,
        INotificationRepository notificationRepository,
        IEmailService emailService,
        ICurrentUserService currentUser,
        ILogger<InterviewScheduleUseCase> logger)
    {
        _interviewRepository = interviewRepository;
        _jobApplicationRepository = jobApplicationRepository;
        _jobRepository = jobRepository;
        _candidateProfileRepository = candidateProfileRepository;
        _employerProfileRepository = employerProfileRepository;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
        _emailService = emailService;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<InterviewScheduleDto> CreateInterviewScheduleAsync(
        CreateInterviewScheduleRequest request,
        CancellationToken cancellationToken = default)
    {
        var employerUserId = AuthorizationGuard.RequireEmployer(_currentUser);

        var application = await _jobApplicationRepository.GetByIdAsync(
            request.JobApplicationId, cancellationToken)
            ?? throw new UseCaseException("Không tìm thấy hồ sơ ứng tuyển.");

        var job = await _jobRepository.GetByIdAsync(application.JobId, cancellationToken)
            ?? throw new UseCaseException("Không tìm thấy tin tuyển dụng.");

        var employerProfile = await _employerProfileRepository.GetByUserIdAsync(
            employerUserId, cancellationToken)
            ?? throw new UseCaseException("Bạn chưa có hồ sơ doanh nghiệp.");

        if (job.EmployerId != employerProfile.Id)
            throw new UseCaseException("Bạn không có quyền tạo lịch phỏng vấn cho job này.");

        var candidateProfile = await _candidateProfileRepository.GetByIdAsync(
            application.CandidateId, cancellationToken)
            ?? throw new UseCaseException("Không tìm thấy hồ sơ ứng viên.");

        var schedule = InterviewSchedule.Create(
            jobApplicationId: request.JobApplicationId,
            employerUserId: employerUserId,
            candidateUserId: candidateProfile.UserId,
            scheduledAt: request.ScheduledAt,
            durationMinutes: request.DurationMinutes,
            interviewType: request.InterviewType,
            meetingLink: request.MeetingLink,
            location: request.Location,
            note: request.Note);

        await _interviewRepository.AddAsync(schedule, cancellationToken);

        _logger.LogInformation(
            "Interview schedule created. ScheduleId={ScheduleId} JobApplicationId={AppId} EmployerUserId={EmployerUserId}",
            schedule.Id, schedule.JobApplicationId, employerUserId);

        // Notification nội bộ cho Candidate
        var notificationMessage = $"Nhà tuyển dụng {employerProfile.CompanyName.Value} đã mời bạn phỏng vấn vị trí {job.Title.Value} vào {schedule.ScheduledAt.ToLocalTime():HH:mm dd/MM/yyyy}.";
        var notification = Notification.Create(
            candidateProfile.UserId,
            "Bạn có lịch phỏng vấn mới",
            notificationMessage,
            NotificationType.System);

        await _notificationRepository.AddAsync(notification, cancellationToken);

        // Email cho Candidate (best-effort)
        await SendEmailBestEffortAsync(
            userId: candidateProfile.UserId,
            subject: "Bạn có lịch phỏng vấn mới từ EnableVN",
            html: EmailTemplates.RenderInterviewInvitationHtml(
                candidateName: candidateProfile.FullName.Value,
                companyName: employerProfile.CompanyName.Value,
                jobTitle: job.Title.Value,
                scheduledAt: schedule.ScheduledAt,
                durationMinutes: schedule.DurationMinutes,
                interviewType: schedule.InterviewType.ToString(),
                meetingLinkOrLocation: schedule.InterviewType == InterviewType.Online
                    ? schedule.MeetingLink
                    : schedule.Location,
                note: schedule.Note),
            cancellationToken);

        return await ToDto(schedule, candidateProfile.FullName.Value, employerProfile.CompanyName.Value, job.Title.Value);
    }

    public async Task AcceptInterviewAsync(Guid interviewScheduleId, CancellationToken cancellationToken = default)
    {
        var candidateUserId = AuthorizationGuard.RequireCandidate(_currentUser);

        var schedule = await _interviewRepository.GetByIdAsync(interviewScheduleId, cancellationToken)
            ?? throw new UseCaseException("Không tìm thấy lịch phỏng vấn.");

        if (schedule.CandidateUserId != candidateUserId)
            throw new UseCaseException("Bạn không có quyền phản hồi lịch phỏng vấn này.");

        schedule.Accept();
        await _interviewRepository.UpdateAsync(schedule, cancellationToken);

        _logger.LogInformation("Interview accepted. ScheduleId={ScheduleId} CandidateUserId={UserId}",
            schedule.Id, candidateUserId);

        await SendInterviewResponseNotificationAsync(schedule, accepted: true, declineReason: null, cancellationToken);
    }

    public async Task DeclineInterviewAsync(Guid interviewScheduleId, string? reason, CancellationToken cancellationToken = default)
    {
        var candidateUserId = AuthorizationGuard.RequireCandidate(_currentUser);

        var schedule = await _interviewRepository.GetByIdAsync(interviewScheduleId, cancellationToken)
            ?? throw new UseCaseException("Không tìm thấy lịch phỏng vấn.");

        if (schedule.CandidateUserId != candidateUserId)
            throw new UseCaseException("Bạn không có quyền phản hồi lịch phỏng vấn này.");

        schedule.Decline(reason);
        await _interviewRepository.UpdateAsync(schedule, cancellationToken);

        _logger.LogInformation("Interview declined. ScheduleId={ScheduleId} CandidateUserId={UserId}",
            schedule.Id, candidateUserId);

        await SendInterviewResponseNotificationAsync(schedule, accepted: false, declineReason: reason, cancellationToken);
    }

    public async Task CancelInterviewAsync(Guid interviewScheduleId, string? reason, CancellationToken cancellationToken = default)
    {
        var employerUserId = AuthorizationGuard.RequireEmployer(_currentUser);

        var schedule = await _interviewRepository.GetByIdAsync(interviewScheduleId, cancellationToken)
            ?? throw new UseCaseException("Không tìm thấy lịch phỏng vấn.");

        if (schedule.EmployerUserId != employerUserId)
            throw new UseCaseException("Bạn không có quyền hủy lịch phỏng vấn này.");

        schedule.Cancel(reason);
        await _interviewRepository.UpdateAsync(schedule, cancellationToken);

        _logger.LogInformation("Interview cancelled. ScheduleId={ScheduleId} EmployerUserId={UserId}",
            schedule.Id, employerUserId);

        // Notify candidate về hủy lịch
        var notification = Notification.Create(
            schedule.CandidateUserId,
            "Lịch phỏng vấn đã bị hủy",
            $"Nhà tuyển dụng đã hủy lịch phỏng vấn của bạn.{(string.IsNullOrWhiteSpace(reason) ? "" : $" Lý do: {reason}")}",
            NotificationType.System);

        await _notificationRepository.AddAsync(notification, cancellationToken);
    }

    public async Task<IReadOnlyList<InterviewScheduleDto>> GetMyInterviewsAsCandidateAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = AuthorizationGuard.RequireCandidate(_currentUser);
        var schedules = await _interviewRepository.GetByCandidateUserIdAsync(userId, cancellationToken);
        return await ToDtoListAsync(schedules, cancellationToken);
    }

    public async Task<IReadOnlyList<InterviewScheduleDto>> GetMyInterviewsAsEmployerAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = AuthorizationGuard.RequireEmployer(_currentUser);
        var schedules = await _interviewRepository.GetByEmployerUserIdAsync(userId, cancellationToken);
        return await ToDtoListAsync(schedules, cancellationToken);
    }

    public async Task<IReadOnlyList<InterviewScheduleDto>> GetByJobApplicationAsync(
        Guid jobApplicationId,
        CancellationToken cancellationToken = default)
    {
        AuthorizationGuard.RequireEmployer(_currentUser);
        var schedules = await _interviewRepository.GetByJobApplicationIdAsync(jobApplicationId, cancellationToken);
        return await ToDtoListAsync(schedules, cancellationToken);
    }

    // ────────── helpers ──────────

    private async Task SendInterviewResponseNotificationAsync(
        InterviewSchedule schedule,
        bool accepted,
        string? declineReason,
        CancellationToken cancellationToken)
    {
        try
        {
            // Lấy thông tin để build notification & email
            var candidateProfile = await _candidateProfileRepository.GetByIdAsync(
                // Lấy theo userId thay vì candidateId — cần tìm qua userId
                Guid.Empty, cancellationToken);

            // Lấy candidate name qua userId
            var candidateUser = await _userRepository.GetByIdAsync(schedule.CandidateUserId, cancellationToken);
            var candidateName = candidateUser?.Email.Value ?? "Ứng viên";

            var application = await _jobApplicationRepository.GetByIdAsync(schedule.JobApplicationId, cancellationToken);
            var job = application is not null
                ? await _jobRepository.GetByIdAsync(application.JobId, cancellationToken)
                : null;

            var employerProfile = await _employerProfileRepository.GetByUserIdAsync(schedule.EmployerUserId, cancellationToken);
            var companyName = employerProfile?.CompanyName.Value ?? "Nhà tuyển dụng";
            var jobTitle = job?.Title.Value ?? "Vị trí ứng tuyển";

            var responseText = accepted ? "đã xác nhận" : "đã từ chối";

            // Notification cho Employer
            var notification = Notification.Create(
                schedule.EmployerUserId,
                "Ứng viên phản hồi lịch phỏng vấn",
                $"Ứng viên {responseText} lịch phỏng vấn vị trí {jobTitle}.",
                NotificationType.System);
            await _notificationRepository.AddAsync(notification, cancellationToken);

            // Email cho Employer (best-effort)
            await SendEmailBestEffortAsync(
                userId: schedule.EmployerUserId,
                subject: $"EnableVN - Ứng viên {responseText} lịch phỏng vấn",
                html: EmailTemplates.RenderInterviewResponseHtml(
                    employerCompanyName: companyName,
                    candidateName: candidateName,
                    jobTitle: jobTitle,
                    accepted: accepted,
                    declineReason: declineReason),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send interview response notification. ScheduleId={ScheduleId}",
                schedule.Id);
        }
    }

    private async Task SendEmailBestEffortAsync(
        Guid userId,
        string subject,
        string html,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            var email = user?.Email.Value;
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Interview email skipped: empty recipient. UserId={UserId}", userId);
                return;
            }

            await _emailService.SendAsync(email, subject, html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Interview email failed (best-effort). UserId={UserId} Subject={Subject}",
                userId, subject);
        }
    }

    private async Task<IReadOnlyList<InterviewScheduleDto>> ToDtoListAsync(
        IReadOnlyList<InterviewSchedule> schedules,
        CancellationToken cancellationToken)
    {
        var result = new List<InterviewScheduleDto>(schedules.Count);
        foreach (var s in schedules)
        {
            var candidateName = string.Empty;
            var companyName = string.Empty;
            var jobTitle = string.Empty;

            try
            {
                var candidateUser = await _userRepository.GetByIdAsync(s.CandidateUserId, cancellationToken);
                candidateName = candidateUser?.Email.Value ?? string.Empty;

                var ep = await _employerProfileRepository.GetByUserIdAsync(s.EmployerUserId, cancellationToken);
                companyName = ep?.CompanyName.Value ?? string.Empty;

                var app = await _jobApplicationRepository.GetByIdAsync(s.JobApplicationId, cancellationToken);
                if (app is not null)
                {
                    var job = await _jobRepository.GetByIdAsync(app.JobId, cancellationToken);
                    jobTitle = job?.Title.Value ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to enrich interview dto. ScheduleId={ScheduleId}", s.Id);
            }

            result.Add(await ToDto(s, candidateName, companyName, jobTitle));
        }

        return result;
    }

    private static Task<InterviewScheduleDto> ToDto(
        InterviewSchedule s,
        string candidateName,
        string companyName,
        string jobTitle)
    {
        var statusVi = s.Status switch
        {
            InterviewStatus.Pending => "Chờ phản hồi",
            InterviewStatus.Accepted => "Đã xác nhận",
            InterviewStatus.Declined => "Đã từ chối",
            InterviewStatus.Cancelled => "Đã hủy",
            InterviewStatus.Completed => "Đã hoàn thành",
            _ => s.Status.ToString()
        };

        return Task.FromResult(new InterviewScheduleDto
        {
            Id = s.Id,
            JobApplicationId = s.JobApplicationId,
            EmployerUserId = s.EmployerUserId,
            CandidateUserId = s.CandidateUserId,
            CandidateName = candidateName,
            EmployerCompanyName = companyName,
            JobTitle = jobTitle,
            ScheduledAt = s.ScheduledAt,
            DurationMinutes = s.DurationMinutes,
            InterviewType = s.InterviewType.ToString(),
            MeetingLink = s.MeetingLink,
            Location = s.Location,
            Note = s.Note,
            Status = s.Status.ToString(),
            StatusVi = statusVi,
            CreatedAt = s.CreatedAt,
            CandidateRespondedAt = s.CandidateRespondedAt,
            CandidateDeclineReason = s.CandidateDeclineReason
        });
    }
}
