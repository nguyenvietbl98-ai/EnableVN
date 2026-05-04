using Domain.Applications;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureInMemory.Repositories
{
    /// <summary>
    /// Repository lưu JobApplication bằng RAM.
    /// 
    /// Dùng cho luồng:
    /// Candidate nộp hồ sơ -> Employer xem hồ sơ -> Employer đổi trạng thái.
    /// </summary>
    public sealed class InMemoryJobApplicationRepository : IJobApplicationRepository
    {
        private readonly List<JobApplication> _applications = new();

        public Task<JobApplication?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            var application = _applications.FirstOrDefault(x => x.Id == id);

            return Task.FromResult(application);
        }

        public Task<IReadOnlyList<JobApplication>> GetByJobIdAsync(
            Guid jobId,
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<JobApplication> result = _applications
                .Where(x => x.JobId == jobId)
                .OrderByDescending(x => x.SubmittedAt)
                .ToList();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<JobApplication>> GetByCandidateIdAsync(
            Guid candidateId,
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<JobApplication> result = _applications
                .Where(x => x.CandidateId == candidateId)
                .OrderByDescending(x => x.SubmittedAt)
                .ToList();

            return Task.FromResult(result);
        }

        public Task<bool> ExistsByJobIdAndCandidateIdAsync(
            Guid jobId,
            Guid candidateId,
            CancellationToken cancellationToken = default
        )
        {
            var exists = _applications.Any(x =>
                x.JobId == jobId &&
                x.CandidateId == candidateId
            );

            return Task.FromResult(exists);
        }

        public Task AddAsync(
            JobApplication application,
            CancellationToken cancellationToken = default
        )
        {
            _applications.Add(application);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(
            JobApplication application,
            CancellationToken cancellationToken = default
        )
        {
            var index = _applications.FindIndex(x => x.Id == application.Id);

            if (index >= 0)
            {
                _applications[index] = application;
            }

            return Task.CompletedTask;
        }
    }
}
