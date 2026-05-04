using Domain.Jobs;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureInMemory.Repositories
{

    /// <summary>
    /// Repository lưu JobPost bằng RAM.
    /// 
    /// Có hỗ trợ search job Published theo keyword, work mode
    /// và các tiêu chí accessibility.
    /// </summary>
    public sealed class InMemoryJobRepository : IJobRepository
    {
        private readonly List<JobPost> _jobs = new();

        public Task<JobPost?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            var job = _jobs.FirstOrDefault(x => x.Id == id);

            return Task.FromResult(job);
        }

        public Task<IReadOnlyList<JobPost>> GetByEmployerIdAsync(
            Guid employerId,
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<JobPost> result = _jobs
                .Where(x => x.EmployerId == employerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<JobPost>> SearchPublishedJobsAsync(
            string? keyword,
            WorkMode? workMode,
            bool? supportsWheelchairAccess,
            bool? supportsRemoteWork,
            bool? supportsFlexibleTime,
            bool? providesAssistiveDevices,
            CancellationToken cancellationToken = default
        )
        {
            var query = _jobs
                .Where(x => x.Status == JobStatus.Published)
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim().ToLower();

                query = query.Where(x =>
                    x.Title.Value.ToLower().Contains(normalizedKeyword) ||
                    x.Description.ToLower().Contains(normalizedKeyword) ||
                    x.Requirement.ToLower().Contains(normalizedKeyword)
                );
            }

            if (workMode.HasValue)
            {
                query = query.Where(x => x.WorkMode == workMode.Value);
            }

            if (supportsWheelchairAccess.HasValue)
            {
                query = query.Where(x =>
                    x.AccessibilityInfo.SupportsWheelchairAccess == supportsWheelchairAccess.Value
                );
            }

            if (supportsRemoteWork.HasValue)
            {
                query = query.Where(x =>
                    x.AccessibilityInfo.SupportsRemoteWork == supportsRemoteWork.Value
                );
            }

            if (supportsFlexibleTime.HasValue)
            {
                query = query.Where(x =>
                    x.AccessibilityInfo.SupportsFlexibleTime == supportsFlexibleTime.Value
                );
            }

            if (providesAssistiveDevices.HasValue)
            {
                query = query.Where(x =>
                    x.AccessibilityInfo.ProvidesAssistiveDevices == providesAssistiveDevices.Value
                );
            }

            IReadOnlyList<JobPost> result = query
                .OrderByDescending(x => x.PublishedAt ?? x.CreatedAt)
                .ToList();

            return Task.FromResult(result);
        }

        public Task AddAsync(
            JobPost job,
            CancellationToken cancellationToken = default
        )
        {
            _jobs.Add(job);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(
            JobPost job,
            CancellationToken cancellationToken = default
        )
        {
            var index = _jobs.FindIndex(x => x.Id == job.Id);

            if (index >= 0)
            {
                _jobs[index] = job;
            }

            return Task.CompletedTask;
        }
    }
}
