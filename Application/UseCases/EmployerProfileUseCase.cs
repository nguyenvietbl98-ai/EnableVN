using Application.Email;
using Application.Common;
using Application.Mappers;
using Domain.Employers;
using Domain.Notifications;
using Domain.Users;
using Microsoft.Extensions.Logging;
using Ports.Inbound;
using Ports.Models.Employers;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases
{
    /// <summary>
    /// UseCase quản lý hồ sơ doanh nghiệp.
    /// 
    /// UserId luôn lấy từ ICurrentUserService, không nhận từ client.
    /// </summary>
    public sealed class EmployerProfileUseCase : IEmployerProfileUseCase
    {
        private readonly IEmployerProfileRepository _employerProfileRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmployerProfileUseCase> _logger;

        public EmployerProfileUseCase(
            IEmployerProfileRepository employerProfileRepository,
            ICurrentUserService currentUser,
            IDomainEventDispatcher domainEventDispatcher,
            INotificationRepository notificationRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            ILogger<EmployerProfileUseCase> logger
        )
        {
            _employerProfileRepository = employerProfileRepository;
            _currentUser = currentUser;
            _domainEventDispatcher = domainEventDispatcher;
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Guid> CreateAsync(
            CreateEmployerProfileCommand command,
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireEmployer(_currentUser);

            var exists = await _employerProfileRepository.ExistsByUserIdAsync(
                userId,
                cancellationToken
            );

            if (exists)
                throw new UseCaseException("Nhà tuyển dụng đã có hồ sơ doanh nghiệp.");

            var workplaceInfo = InclusiveWorkplaceInfo.Create(
                command.HasWheelchairAccess,
                command.HasAccessibleRestroom,
                command.SupportsFlexibleWorkingTime,
                command.SupportsRemoteWork,
                command.ProvidesAssistiveDevices
            );

            var profile = EmployerProfile.Create(
                userId,
                command.CompanyName,
                command.Description,
                command.WebsiteUrl,
                workplaceInfo
            );
            profile.UpdateCompanyInfo(
                command.CompanyName,
                command.LogoUrl,
                command.ContactEmail,
                command.PhoneNumber,
                command.Address,
                command.CompanySize,
                command.Industry,
                command.TaxCode,
                command.RecruiterContactName,
                command.RecruiterContactTitle,
                command.Description,
                command.Benefits,
                command.Culture,
                command.WebsiteUrl
            );

            await _employerProfileRepository.AddAsync(profile, cancellationToken);

            await DomainEventHelper.DispatchAndClearEventsAsync(
                profile,
                _domainEventDispatcher,
                cancellationToken
            );

            return profile.Id;
        }

        public async Task UpdateMyProfileAsync(
            UpdateEmployerProfileCommand command,
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireEmployer(_currentUser);

            var profile = await _employerProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken
            );

            if (profile is null)
                throw new UseCaseException("Bạn chưa có hồ sơ doanh nghiệp.");

            var workplaceInfo = InclusiveWorkplaceInfo.Create(
                command.HasWheelchairAccess,
                command.HasAccessibleRestroom,
                command.SupportsFlexibleWorkingTime,
                command.SupportsRemoteWork,
                command.ProvidesAssistiveDevices
            );

            profile.UpdateCompanyInfo(
                command.CompanyName,
                command.LogoUrl,
                command.ContactEmail,
                command.PhoneNumber,
                command.Address,
                command.CompanySize,
                command.Industry,
                command.TaxCode,
                command.RecruiterContactName,
                command.RecruiterContactTitle,
                command.Description,
                command.Benefits,
                command.Culture,
                command.WebsiteUrl
            );

            profile.UpdateInclusiveWorkplaceInfo(workplaceInfo);

            await _employerProfileRepository.UpdateAsync(profile, cancellationToken);
        }

        public async Task<EmployerProfileResult?> GetMyProfileAsync(
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireEmployer(_currentUser);

            var profile = await _employerProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken
            );

            return profile is null
                ? null
                : EmployerProfileMapper.ToResult(profile);
        }

        public async Task<EmployerProfileResult?> GetByIdAsync(
            Guid employerProfileId,
            CancellationToken cancellationToken = default
        )
        {
            var profile = await _employerProfileRepository.GetByIdAsync(
                employerProfileId,
                cancellationToken
            );

            return profile is null
                ? null
                : EmployerProfileMapper.ToResult(profile);
        }

        public async Task<IReadOnlyList<EmployerProfileReviewItemResult>> GetPendingProfilesAsync(
            CancellationToken cancellationToken = default)
        {
            AuthorizationGuard.RequireAdmin(_currentUser);
            var all = await GetProfilesForReviewAsync(cancellationToken);
            return all.Where(x => x.VerificationStatus == EmployerVerificationStatus.Pending.ToString()).ToList();
        }

        public async Task<IReadOnlyList<EmployerProfileReviewItemResult>> GetProfilesForReviewAsync(
            CancellationToken cancellationToken = default)
        {
            AuthorizationGuard.RequireAdmin(_currentUser);
            var profiles = await _employerProfileRepository.GetAllAsync(cancellationToken);
            return profiles
                .Select(x => new EmployerProfileReviewItemResult
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    CompanyName = x.CompanyName.Value,
                    Industry = x.Industry,
                    ContactEmail = x.ContactEmail,
                    PhoneNumber = x.PhoneNumber,
                    VerificationStatus = x.VerificationStatus.ToString(),
                    VerifiedAtUtc = x.VerifiedAtUtc,
                    VerificationNote = x.VerificationNote
                })
                .OrderByDescending(x => x.VerificationStatus == EmployerVerificationStatus.Pending.ToString())
                .ThenByDescending(x => x.VerifiedAtUtc ?? DateTime.MinValue)
                .ToList();
        }

        public async Task ApproveProfileAsync(Guid employerProfileId, string? note, CancellationToken cancellationToken = default)
        {
            AuthorizationGuard.RequireAdmin(_currentUser);
            var profile = await _employerProfileRepository.GetByIdAsync(employerProfileId, cancellationToken)
                ?? throw new UseCaseException("Không tìm thấy hồ sơ doanh nghiệp.");
            profile.ApproveByAdmin(note);
            await _employerProfileRepository.UpdateAsync(profile, cancellationToken);

            // FLOW 2 — tạo notification trong DB + gửi email best-effort
            var notification = Notification.Create(
                profile.UserId,
                "Hồ sơ doanh nghiệp đã được duyệt",
                string.IsNullOrWhiteSpace(note)
                    ? $"Hồ sơ doanh nghiệp \"{profile.CompanyName.Value}\" đã được duyệt."
                    : $"Hồ sơ doanh nghiệp \"{profile.CompanyName.Value}\" đã được duyệt. Ghi chú: {note}",
                NotificationType.System);

            await _notificationRepository.AddAsync(notification, cancellationToken);

            await SendEmployerReviewEmailBestEffortAsync(
                profileUserId: profile.UserId,
                companyName: profile.CompanyName.Value,
                approved: true,
                reasonOrNote: note,
                cancellationToken: cancellationToken);
        }

        public async Task RejectProfileAsync(Guid employerProfileId, string? note, CancellationToken cancellationToken = default)
        {
            AuthorizationGuard.RequireAdmin(_currentUser);
            var profile = await _employerProfileRepository.GetByIdAsync(employerProfileId, cancellationToken)
                ?? throw new UseCaseException("Không tìm thấy hồ sơ doanh nghiệp.");
            profile.RejectByAdmin(note);
            await _employerProfileRepository.UpdateAsync(profile, cancellationToken);

            // FLOW 2 — tạo notification trong DB + gửi email best-effort
            var notification = Notification.Create(
                profile.UserId,
                "Hồ sơ doanh nghiệp bị từ chối",
                string.IsNullOrWhiteSpace(note)
                    ? $"Hồ sơ doanh nghiệp \"{profile.CompanyName.Value}\" đã bị từ chối."
                    : $"Hồ sơ doanh nghiệp \"{profile.CompanyName.Value}\" đã bị từ chối. Lý do: {note}",
                NotificationType.System);

            await _notificationRepository.AddAsync(notification, cancellationToken);

            await SendEmployerReviewEmailBestEffortAsync(
                profileUserId: profile.UserId,
                companyName: profile.CompanyName.Value,
                approved: false,
                reasonOrNote: note,
                cancellationToken: cancellationToken);
        }

        private async Task SendEmployerReviewEmailBestEffortAsync(
            Guid profileUserId,
            string companyName,
            bool approved,
            string? reasonOrNote,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(profileUserId, cancellationToken);
                var recipient = user?.Email.Value;

                if (string.IsNullOrWhiteSpace(recipient))
                {
                    _logger.LogWarning(
                        "Employer review email skipped: recipient is empty. ProfileUserId={ProfileUserId}",
                        profileUserId);
                    return;
                }

                var subject = approved
                    ? "EnableVN - Hồ sơ doanh nghiệp đã được duyệt"
                    : "EnableVN - Hồ sơ doanh nghiệp bị từ chối";

                var html = EmailTemplates.RenderEmployerProfileReviewedHtml(
                    companyName: companyName,
                    approved: approved,
                    reasonOrNote: reasonOrNote);

                await _emailService.SendAsync(recipient, subject, html, cancellationToken);
            }
            catch (Exception ex)
            {
                // Best-effort: không làm fail luồng chính.
                _logger.LogWarning(
                    ex,
                    "Failed to send employer review email. ProfileUserId={ProfileUserId} Approved={Approved}",
                    profileUserId,
                    approved);
            }
        }
    }
}
