using Application.Common;
using Application.Mappers;
using Domain.Applications;
using Ports.Inbound;
using Ports.Models.Applications;
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
        private readonly ICurrentUserService _currentUser;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public JobApplicationUseCase(
            IJobApplicationRepository jobApplicationRepository,
            IJobRepository jobRepository,
            ICandidateProfileRepository candidateProfileRepository,
            IEmployerProfileRepository employerProfileRepository,
            ICurrentUserService currentUser,
            IDomainEventDispatcher domainEventDispatcher
        )
        {
            _jobApplicationRepository = jobApplicationRepository;
            _jobRepository = jobRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _employerProfileRepository = employerProfileRepository;
            _currentUser = currentUser;
            _domainEventDispatcher = domainEventDispatcher;
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

            application.ChangeStatus(
                command.NewStatus,
                command.Note
            );

            await _jobApplicationRepository.UpdateAsync(
                application,
                cancellationToken
            );

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
    }
}
