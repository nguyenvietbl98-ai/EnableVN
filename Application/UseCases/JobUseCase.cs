using Application.Common;
using Application.Mappers;
using Domain.Jobs;
using Ports.Inbound;
using Ports.Models.Jobs;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases
{
    /// <summary>
    /// UseCase quản lý tin tuyển dụng.
    /// 
    /// Đây là use case trung tâm của MVP:
    /// Employer tạo job, publish job, Candidate tìm job.
    /// </summary>
    public sealed class JobUseCase : IJobUseCase
    {
        private readonly IJobRepository _jobRepository;
        private readonly IEmployerProfileRepository _employerProfileRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public JobUseCase(
            IJobRepository jobRepository,
            IEmployerProfileRepository employerProfileRepository,
            ICurrentUserService currentUser,
            IDomainEventDispatcher domainEventDispatcher
        )
        {
            _jobRepository = jobRepository;
            _employerProfileRepository = employerProfileRepository;
            _currentUser = currentUser;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public async Task<Guid> CreateDraftAsync(
            CreateJobCommand command,
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireEmployer(_currentUser);

            var employerProfile = await _employerProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken
            );

            if (employerProfile is null)
                throw new UseCaseException("Bạn cần tạo hồ sơ doanh nghiệp trước khi đăng tin.");

            var salaryRange = SalaryRange.Create(
                command.MinSalary,
                command.MaxSalary
            );

            var accessibilityInfo = JobAccessibilityInfo.Create(
                command.SupportsWheelchairAccess,
                command.SupportsRemoteWork,
                command.SupportsFlexibleTime,
                command.ProvidesAssistiveDevices,
                command.AdditionalSupportDescription
            );

            var job = JobPost.CreateDraft(
                employerProfile.Id,
                command.Title,
                command.Description,
                command.Requirement,
                command.WorkMode,
                salaryRange,
                accessibilityInfo
            );

            await _jobRepository.AddAsync(job, cancellationToken);

            return job.Id;
        }

        public async Task UpdateAsync(
            UpdateJobCommand command,
            CancellationToken cancellationToken = default
        )
        {
            var job = await GetMyJobOrThrowAsync(
                command.JobId,
                cancellationToken
            );

            var salaryRange = SalaryRange.Create(
                command.MinSalary,
                command.MaxSalary
            );

            var accessibilityInfo = JobAccessibilityInfo.Create(
                command.SupportsWheelchairAccess,
                command.SupportsRemoteWork,
                command.SupportsFlexibleTime,
                command.ProvidesAssistiveDevices,
                command.AdditionalSupportDescription
            );

            job.UpdateContent(
                command.Title,
                command.Description,
                command.Requirement,
                command.WorkMode,
                salaryRange,
                accessibilityInfo
            );

            await _jobRepository.UpdateAsync(job, cancellationToken);
        }

        public async Task PublishAsync(
            Guid jobId,
            CancellationToken cancellationToken = default
        )
        {
            var job = await GetMyJobOrThrowAsync(
                jobId,
                cancellationToken
            );

            job.Publish();

            await _jobRepository.UpdateAsync(job, cancellationToken);

            await DomainEventHelper.DispatchAndClearEventsAsync(
                job,
                _domainEventDispatcher,
                cancellationToken
            );
        }

        public async Task CloseAsync(
            Guid jobId,
            CancellationToken cancellationToken = default
        )
        {
            var job = await GetMyJobOrThrowAsync(
                jobId,
                cancellationToken
            );

            job.Close();

            await _jobRepository.UpdateAsync(job, cancellationToken);

            await DomainEventHelper.DispatchAndClearEventsAsync(
                job,
                _domainEventDispatcher,
                cancellationToken
            );
        }

        public async Task DeleteAsync(
            Guid jobId,
            CancellationToken cancellationToken = default
        )
        {
            var job = await GetMyJobOrThrowAsync(
                jobId,
                cancellationToken
            );

            job.Delete();

            await _jobRepository.UpdateAsync(job, cancellationToken);
        }

        public async Task<JobResult?> GetByIdAsync(
            Guid jobId,
            CancellationToken cancellationToken = default
        )
        {
            var job = await _jobRepository.GetByIdAsync(
                jobId,
                cancellationToken
            );

            if (job is null)
                return null;

            return JobMapper.ToResult(job);
        }

        public async Task<IReadOnlyList<JobResult>> GetMyJobsAsync(
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

            var jobs = await _jobRepository.GetByEmployerIdAsync(
                employerProfile.Id,
                cancellationToken
            );

            return jobs
                .Select(JobMapper.ToResult)
                .ToList();
        }

        public async Task<IReadOnlyList<JobResult>> SearchPublishedJobsAsync(
            SearchJobQuery query,
            CancellationToken cancellationToken = default
        )
        {
            var jobs = await _jobRepository.SearchPublishedJobsAsync(
                query.Keyword,
                query.WorkMode,
                query.SupportsWheelchairAccess,
                query.SupportsRemoteWork,
                query.SupportsFlexibleTime,
                query.ProvidesAssistiveDevices,
                cancellationToken
            );

            return jobs
                .Select(JobMapper.ToResult)
                .ToList();
        }

        /// <summary>
        /// Lấy job và đảm bảo job thuộc Employer hiện tại.
        /// 
        /// Rule này nằm ở Application vì cần:
        /// - current user
        /// - employer profile repository
        /// - job repository
        /// </summary>
        private async Task<JobPost> GetMyJobOrThrowAsync(
            Guid jobId,
            CancellationToken cancellationToken
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
                throw new UseCaseException("Bạn không có quyền thao tác với tin tuyển dụng này.");

            return job;
        }
    }
}
