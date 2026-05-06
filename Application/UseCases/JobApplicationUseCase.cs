using Application.Email;
using Application.Common;
using Application.Mappers;
using Domain.Applications;
using Domain.Notifications;
using Domain.Users;
using Microsoft.Extensions.Logging;
using Ports.Inbound;
using Ports.Models.Applications;
using Ports.Models.Chat;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases
{
    /// <summary>
    /// UseCase quản lý hồ sơ ứng tuyển.
    /// 
    /// Đây là luồng chính:
    /// Candidate nộp CV -> Employer đổi trạng thái hồ sơ.
    /// </summary>
    public sealed class JobApplicationUseCase : IJobApplicationUseCase
    {
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IJobRepository _jobRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IEmployerProfileRepository _employerProfileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<JobApplicationUseCase> _logger;

        public JobApplicationUseCase(
            IJobApplicationRepository jobApplicationRepository,
            IJobRepository jobRepository,
            ICandidateProfileRepository candidateProfileRepository,
            IEmployerProfileRepository employerProfileRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUser,
            IDomainEventDispatcher domainEventDispatcher,
            INotificationRepository notificationRepository,
            IEmailService emailService,
            ILogger<JobApplicationUseCase> logger
        )
        {
            _jobApplicationRepository = jobApplicationRepository;
            _jobRepository = jobRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _employerProfileRepository = employerProfileRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
            _domainEventDispatcher = domainEventDispatcher;
            _notificationRepository = notificationRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Guid> SubmitAsync(
            SubmitJobApplicationCommand command,
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireCandidate(_currentUser);

            var candidateProfile = await _candidateProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken
            );

            if (candidateProfile is null)
                throw new UseCaseException("Bạn cần tạo hồ sơ ứng viên trước khi nộp hồ sơ.");

            var job = await _jobRepository.GetByIdAsync(
                command.JobId,
                cancellationToken
            );

            if (job is null)
                throw new UseCaseException("Không tìm thấy tin tuyển dụng.");

            if (!job.CanReceiveApplication())
                throw new UseCaseException("Tin tuyển dụng hiện không nhận hồ sơ.");

            var jobEmployerProfile = await _employerProfileRepository.GetByIdAsync(
                job.EmployerId,
                cancellationToken);

            if (jobEmployerProfile is not null && jobEmployerProfile.UserId == userId)
                throw new UseCaseException("Bạn không thể nộp hồ sơ vào tin do chính doanh nghiệp của bạn đăng.");

            var alreadyApplied = await _jobApplicationRepository.ExistsByJobIdAndCandidateIdAsync(
                job.Id,
                candidateProfile.Id,
                cancellationToken
            );

            if (alreadyApplied)
                throw new UseCaseException("Bạn đã nộp hồ sơ vào công việc này.");

            var cvUrl = string.IsNullOrWhiteSpace(command.CvUrl)
                ? candidateProfile.CvUrl
                : command.CvUrl;

            var application = JobApplication.Submit(
                job.Id,
                candidateProfile.Id,
                command.CoverLetter,
                cvUrl
            );

            await _jobApplicationRepository.AddAsync(
                application,
                cancellationToken
            );

            // Job đang lưu EmployerId là Id của EmployerProfile.
            if (jobEmployerProfile is not null)
            {
                var notification = Notification.Create(
                    jobEmployerProfile.UserId,
                    "Có hồ sơ ứng tuyển mới",
                    $"Một ứng viên vừa nộp hồ sơ vào tin: {job.Title.Value}.",
                    NotificationType.ApplicationSubmitted
                );
                // Tạo thông báo cho tài khoản Employer sở hữu công việc này.

                await _notificationRepository.AddAsync(
                    notification,
                    cancellationToken
                );

                await SendNotificationEmailBestEffortAsync(
                    userId: jobEmployerProfile.UserId,
                    subject: notification.Title,
                    htmlBody: EmailTemplates.RenderNotificationHtml(notification.Title, notification.Message),
                    cancellationToken
                );
            }

            await DomainEventHelper.DispatchAndClearEventsAsync(
                application,
                _domainEventDispatcher,
                cancellationToken
            );

            return application.Id;
        }

        public async Task ChangeStatusAsync(
            ChangeApplicationStatusCommand command,
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireEmployer(_currentUser);

            var employerProfile = await _employerProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken
            );

            if (employerProfile is null)
                throw new UseCaseException("Bạn chưa có hồ sơ doanh nghiệp.");

            var application = await _jobApplicationRepository.GetByIdAsync(
                command.ApplicationId,
                cancellationToken
            );

            if (application is null)
                throw new UseCaseException("Không tìm thấy hồ sơ ứng tuyển.");

            var job = await _jobRepository.GetByIdAsync(
                application.JobId,
                cancellationToken
            );

            if (job is null)
                throw new UseCaseException("Không tìm thấy tin tuyển dụng của hồ sơ này.");

            if (job.EmployerId != employerProfile.Id)
                throw new UseCaseException("Bạn không có quyền đổi trạng thái hồ sơ này.");

            var previousStatus = application.Status;

            application.ChangeStatus(
                command.NewStatus,
                command.Note
            );

            await _jobApplicationRepository.UpdateAsync(
                application,
                cancellationToken
            );
            var candidateProfile = await _candidateProfileRepository.GetByIdAsync(
    application.CandidateId,
    cancellationToken
);
            // Lấy CandidateProfile để biết UserId của ứng viên nhận thông báo.

            if (candidateProfile is not null)
            {
                var statusChanged = previousStatus != application.Status;
                var statusVi = EmailTemplates.ToVietnameseApplicationStatus(application.Status);
                var notificationMessage = statusChanged
                    ? $"Hồ sơ của bạn đã được cập nhật sang trạng thái: {statusVi}."
                    : "Nhà tuyển dụng vừa gửi phản hồi (ghi chú) cho hồ sơ ứng tuyển của bạn — xem trong mục Đơn ứng tuyển.";

                var notification = Notification.Create(
                    candidateProfile.UserId,
                    statusChanged ? "Trạng thái hồ sơ đã thay đổi" : "Phản hồi từ nhà tuyển dụng",
                    notificationMessage,
                    NotificationType.ApplicationStatusChanged
                );
                // Tạo thông báo cho Candidate.

                await _notificationRepository.AddAsync(
                    notification,
                    cancellationToken
                );

                await SendNotificationEmailBestEffortAsync(
                    userId: candidateProfile.UserId,
                    subject: statusChanged ? "EnableVN - Cập nhật hồ sơ ứng tuyển" : "EnableVN - Phản hồi từ nhà tuyển dụng",
                    htmlBody: statusChanged
                        ? EmailTemplates.RenderApplicationStatusChangedHtml(job.Title.Value, application.Status, command.Note)
                        : EmailTemplates.RenderNotificationHtml(notification.Title, string.IsNullOrWhiteSpace(command.Note) ? notification.Message : command.Note),
                    cancellationToken
                );
            }

            await DomainEventHelper.DispatchAndClearEventsAsync(
                application,
                _domainEventDispatcher,
                cancellationToken
            );
        }

        public async Task WithdrawAsync(
            Guid applicationId,
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireCandidate(_currentUser);

            var candidateProfile = await _candidateProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken
            );

            if (candidateProfile is null)
                throw new UseCaseException("Bạn chưa có hồ sơ ứng viên.");

            var application = await _jobApplicationRepository.GetByIdAsync(
                applicationId,
                cancellationToken
            );

            if (application is null)
                throw new UseCaseException("Không tìm thấy hồ sơ ứng tuyển.");

            if (application.CandidateId != candidateProfile.Id)
                throw new UseCaseException("Bạn không có quyền rút hồ sơ này.");

            application.Withdraw();

            await _jobApplicationRepository.UpdateAsync(
                application,
                cancellationToken
            );
        }

        public async Task<IReadOnlyList<JobApplicationResult>> GetByJobIdAsync(
            Guid jobId,
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireEmployer(_currentUser);

            var employerProfile = await _employerProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken
            );

            if (employerProfile is null)
                throw new UseCaseException("Bạn chưa có hồ sơ doanh nghiệp.");

            var job = await _jobRepository.GetByIdAsync(
                jobId,
                cancellationToken
            );

            if (job is null)
                throw new UseCaseException("Không tìm thấy tin tuyển dụng.");

            if (job.EmployerId != employerProfile.Id)
                throw new UseCaseException("Bạn không có quyền xem hồ sơ của tin này.");

            var applications = await _jobApplicationRepository.GetByJobIdAsync(
                jobId,
                cancellationToken
            );

            return applications
                .Select(JobApplicationMapper.ToResult)
                .ToList();
        }

        public async Task<IReadOnlyList<JobApplicationResult>> GetMyApplicationsAsync(
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireCandidate(_currentUser);

            var candidateProfile = await _candidateProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken
            );

            if (candidateProfile is null)
                throw new UseCaseException("Bạn chưa có hồ sơ ứng viên.");

            var applications = await _jobApplicationRepository.GetByCandidateIdAsync(
                candidateProfile.Id,
                cancellationToken
            );

            return applications
                .Select(JobApplicationMapper.ToResult)
                .ToList();
        }

        public async Task<JobApplicationResult?> GetByIdAsync(
            Guid applicationId,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAuthenticatedUser(_currentUser);

            var application = await _jobApplicationRepository.GetByIdAsync(
                applicationId,
                cancellationToken
            );

            if (application is null)
                return null;

            return JobApplicationMapper.ToResult(application);
        }

        public async Task<Guid?> TryGetCurrentCandidateApplicationIdForJobAsync(
            Guid jobId,
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireCandidate(_currentUser);

            var candidateProfile = await _candidateProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken
            );

            if (candidateProfile is null)
                return null;

            var applications = await _jobApplicationRepository.GetByCandidateIdAsync(
                candidateProfile.Id,
                cancellationToken
            );

            var match = applications.FirstOrDefault(a => a.JobId == jobId);
            if (match is null || match.Status == ApplicationStatus.Withdrawn)
                return null;

            return match.Id;
        }

        public async Task EnsureCurrentUserCanChatOnApplicationAsync(
            Guid applicationId,
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireAuthenticatedUser(_currentUser);
            var role = _currentUser.Role
                ?? throw new UseCaseException("Bạn cần đăng nhập để thực hiện thao tác này.");

            var application = await _jobApplicationRepository.GetByIdAsync(
                applicationId,
                cancellationToken
            );

            if (application is null)
                throw new UseCaseException("Không tìm thấy hồ sơ ứng tuyển.");

            if (application.Status == ApplicationStatus.Withdrawn)
                throw new UseCaseException("Hồ sơ đã rút — không thể tiếp tục trò chuyện.");

            if (role == UserRole.Candidate)
            {
                var candidateProfile = await _candidateProfileRepository.GetByUserIdAsync(
                    userId,
                    cancellationToken
                );

                if (candidateProfile is null || application.CandidateId != candidateProfile.Id)
                    throw new UseCaseException("Bạn không có quyền mở cuộc trò chuyện này.");

                return;
            }

            if (role == UserRole.Employer)
            {
                var employerProfile = await _employerProfileRepository.GetByUserIdAsync(
                    userId,
                    cancellationToken
                );

                if (employerProfile is null)
                    throw new UseCaseException("Bạn chưa có hồ sơ doanh nghiệp.");

                var job = await _jobRepository.GetByIdAsync(
                    application.JobId,
                    cancellationToken
                );

                if (job is null || job.EmployerId != employerProfile.Id)
                    throw new UseCaseException("Bạn không có quyền mở cuộc trò chuyện này.");

                return;
            }

            throw new UseCaseException("Chỉ ứng viên hoặc nhà tuyển dụng liên quan mới dùng được chat.");
        }

        public async Task<ApplicationChatThreadDto> GetChatThreadForCurrentUserAsync(
            Guid applicationId,
            CancellationToken cancellationToken = default
        )
        {
            await EnsureCurrentUserCanChatOnApplicationAsync(applicationId, cancellationToken);

            var application = await _jobApplicationRepository.GetByIdAsync(
                applicationId,
                cancellationToken
            );

            if (application is null)
                throw new UseCaseException("Không tìm thấy hồ sơ ứng tuyển.");

            var job = await _jobRepository.GetByIdAsync(
                application.JobId,
                cancellationToken
            );

            if (job is null)
                throw new UseCaseException("Không tìm thấy tin tuyển dụng.");

            return new ApplicationChatThreadDto
            {
                ApplicationId = application.Id,
                JobId = application.JobId,
                JobTitle = job.Title.Value
            };
        }

        private async Task SendNotificationEmailBestEffortAsync(
            Guid userId,
            string subject,
            string htmlBody,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
                var recipient = user?.Email.Value;
                if (string.IsNullOrWhiteSpace(recipient))
                {
                    _logger.LogWarning(
                        "Notification email skipped: recipient is empty. UserId={UserId} Subject={Subject}",
                        userId,
                        subject);
                    return;
                }

                await _emailService.SendAsync(
                    recipient,
                    subject,
                    htmlBody,
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                // MVP: email fail không được làm fail nghiệp vụ chính.
                _logger.LogWarning(
                    ex,
                    "Notification email failed (best-effort). UserId={UserId} Subject={Subject}",
                    userId,
                    subject);
            }
        }
    }
}
