using Domain.Catalogs;
using InfrastructureSqlite.Mappers;
using InfrastructureSqlite.Persistence;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureSqlite.Repositories
{
    public sealed class SqliteJobCategoryRepository : IJobCategoryRepository
    {
        private readonly EnableVnDbContext _dbContext;

        public SqliteJobCategoryRepository(EnableVnDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<JobCategory?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.JobCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return record is null
                ? null
                : CatalogPersistenceMapper.ToDomainJobCategory(record);
        }

        public async Task<IReadOnlyList<JobCategory>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var records = await _dbContext.JobCategories
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);

            return records
                .Select(CatalogPersistenceMapper.ToDomainJobCategory)
                .ToList();
        }

        public async Task<IReadOnlyList<JobCategory>> GetActiveAsync(
            CancellationToken cancellationToken = default)
        {
            var activeStatus = CatalogStatus.Active.ToString();

            var records = await _dbContext.JobCategories
                .AsNoTracking()
                .Where(x => x.Status == activeStatus)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);

            return records
                .Select(CatalogPersistenceMapper.ToDomainJobCategory)
                .ToList();
        }

        public async Task AddAsync(
            JobCategory category,
            CancellationToken cancellationToken = default)
        {
            var record = CatalogPersistenceMapper.ToRecord(category);

            await _dbContext.JobCategories.AddAsync(record, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            JobCategory category,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.JobCategories
                .FirstOrDefaultAsync(x => x.Id == category.Id, cancellationToken);

            if (record is null)
            {
                throw new InvalidOperationException("Không tìm thấy JobCategory để cập nhật.");
            }

            CatalogPersistenceMapper.UpdateRecord(record, category);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
