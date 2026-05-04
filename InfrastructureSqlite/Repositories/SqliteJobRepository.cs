using Domain.Jobs;
using InfrastructureSqlite.Mappers;
using InfrastructureSqlite.Persistence;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureSqlite.Repositories
{
    public sealed class SqliteJobRepository : IJobRepository
    {
        private readonly EnableVnDbContext _dbContext;

        public SqliteJobRepository(EnableVnDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<JobPost?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.JobPosts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return record is null
                ? null
                : JobPostPersistenceMapper.ToDomain(record);
        }

        public async Task<IReadOnlyList<JobPost>> GetByEmployerIdAsync(
            Guid employerId,
            CancellationToken cancellationToken = default)
        {
            var records = await _dbContext.JobPosts
                .AsNoTracking()
                .Where(x => x.EmployerId == employerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return records
                .Select(JobPostPersistenceMapper.ToDomain)
                .ToList();
        }

        public async Task<IReadOnlyList<JobPost>> SearchPublishedJobsAsync(
            string? keyword,
            WorkMode? workMode,
            bool? supportsWheelchairAccess,
            bool? supportsRemoteWork,
            bool? supportsFlexibleTime,
            bool? providesAssistiveDevices,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.JobPosts
                .AsNoTracking()
                .Where(x => x.Status == JobStatus.Published.ToString());

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim().ToLower();

                query = query.Where(x =>
                    x.Title.ToLower().Contains(normalizedKeyword) ||
                    x.Description.ToLower().Contains(normalizedKeyword) ||
                    x.Requirement.ToLower().Contains(normalizedKeyword)
                );
            }

            if (workMode.HasValue)
            {
                var modeString = workMode.Value.ToString();
                query = query.Where(x => x.WorkMode == modeString);
            }

            if (supportsWheelchairAccess.HasValue)
            {
                query = query.Where(x => x.SupportsWheelchairAccess == supportsWheelchairAccess.Value);
            }

            if (supportsRemoteWork.HasValue)
            {
                query = query.Where(x => x.SupportsRemoteWork == supportsRemoteWork.Value);
            }

            if (supportsFlexibleTime.HasValue)
            {
                query = query.Where(x => x.SupportsFlexibleTime == supportsFlexibleTime.Value);
            }

            if (providesAssistiveDevices.HasValue)
            {
                query = query.Where(x => x.ProvidesAssistiveDevices == providesAssistiveDevices.Value);
            }

            var records = await query
                .OrderByDescending(x => x.PublishedAt)
                .ToListAsync(cancellationToken);

            return records
                .Select(JobPostPersistenceMapper.ToDomain)
                .ToList();
        }

        public async Task AddAsync(
            JobPost jobPost,
            CancellationToken cancellationToken = default)
        {
            var record = JobPostPersistenceMapper.ToRecord(jobPost);

            await _dbContext.JobPosts.AddAsync(record, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            JobPost jobPost,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.JobPosts
                .FirstOrDefaultAsync(x => x.Id == jobPost.Id, cancellationToken);

            if (record is null)
            {
                throw new InvalidOperationException("Không tìm thấy JobPost để cập nhật.");
            }

            JobPostPersistenceMapper.UpdateRecord(record, jobPost);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
