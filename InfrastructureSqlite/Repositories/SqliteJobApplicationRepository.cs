using Domain.Applications;
using InfrastructureSqlite.Mappers;
using InfrastructureSqlite.Persistence;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureSqlite.Repositories
{
    public sealed class SqliteJobApplicationRepository : IJobApplicationRepository
    {
        private readonly EnableVnDbContext _dbContext;

        public SqliteJobApplicationRepository(EnableVnDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<JobApplication?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.JobApplications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (record is null)
                return null;

            var statusHistories = await LoadStatusHistoriesAsync(record.Id, cancellationToken);
            return JobApplicationPersistenceMapper.ToDomain(record, statusHistories);
        }

        public async Task<IReadOnlyList<JobApplication>> GetByJobIdAsync(
            Guid jobId,
            CancellationToken cancellationToken = default)
        {
            var records = await _dbContext.JobApplications
                .AsNoTracking()
                .Where(x => x.JobId == jobId)
                .OrderByDescending(x => x.SubmittedAt)
                .ToListAsync(cancellationToken);

            var applications = new List<JobApplication>();

            foreach (var record in records)
            {
                var statusHistories = await LoadStatusHistoriesAsync(record.Id, cancellationToken);
                applications.Add(JobApplicationPersistenceMapper.ToDomain(record, statusHistories));
            }

            return applications;
        }

        public async Task<IReadOnlyList<JobApplication>> GetByCandidateIdAsync(
            Guid candidateId,
            CancellationToken cancellationToken = default)
        {
            var records = await _dbContext.JobApplications
                .AsNoTracking()
                .Where(x => x.CandidateId == candidateId)
                .OrderByDescending(x => x.SubmittedAt)
                .ToListAsync(cancellationToken);

            var applications = new List<JobApplication>();

            foreach (var record in records)
            {
                var statusHistories = await LoadStatusHistoriesAsync(record.Id, cancellationToken);
                applications.Add(JobApplicationPersistenceMapper.ToDomain(record, statusHistories));
            }

            return applications;
        }

        public async Task<bool> ExistsByJobIdAndCandidateIdAsync(
            Guid jobId,
            Guid candidateId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.JobApplications
                .AnyAsync(x => x.JobId == jobId && x.CandidateId == candidateId, cancellationToken);
        }

        public async Task AddAsync(
            JobApplication application,
            CancellationToken cancellationToken = default)
        {
            var record = JobApplicationPersistenceMapper.ToRecord(application);

            await _dbContext.JobApplications.AddAsync(record, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Lưu status histories
            foreach (var history in application.StatusHistories)
            {
                var historyRecord = ApplicationStatusHistoryPersistenceMapper.ToRecord(application.Id, history);
                await _dbContext.ApplicationStatusHistories.AddAsync(historyRecord, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            JobApplication application,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.JobApplications
                .FirstOrDefaultAsync(x => x.Id == application.Id, cancellationToken);

            if (record is null)
            {
                throw new InvalidOperationException("Không tìm thấy JobApplication để cập nhật.");
            }

            JobApplicationPersistenceMapper.UpdateRecord(record, application);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Cập nhật status histories
            var existingHistories = await _dbContext.ApplicationStatusHistories
                .Where(x => x.JobApplicationId == application.Id)
                .ToListAsync(cancellationToken);

            // Xóa histories cũ
            _dbContext.ApplicationStatusHistories.RemoveRange(existingHistories);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Thêm histories mới
            foreach (var history in application.StatusHistories)
            {
                var historyRecord = ApplicationStatusHistoryPersistenceMapper.ToRecord(application.Id, history);
                await _dbContext.ApplicationStatusHistories.AddAsync(historyRecord, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<IReadOnlyList<ApplicationStatusHistory>> LoadStatusHistoriesAsync(
            Guid jobApplicationId,
            CancellationToken cancellationToken)
        {
            var historyRecords = await _dbContext.ApplicationStatusHistories
                .AsNoTracking()
                .Where(x => x.JobApplicationId == jobApplicationId)
                .OrderBy(x => x.ChangedAt)
                .ToListAsync(cancellationToken);

            return historyRecords
                .Select(ApplicationStatusHistoryPersistenceMapper.ToDomain)
                .ToList();
        }
    }
}
