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
    public sealed class SqliteDisabilityTypeRepository : IDisabilityTypeRepository
    {
        private readonly EnableVnDbContext _dbContext;

        public SqliteDisabilityTypeRepository(EnableVnDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DisabilityType?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.DisabilityTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return record is null
                ? null
                : CatalogPersistenceMapper.ToDomainDisabilityType(record);
        }

        public async Task<IReadOnlyList<DisabilityType>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var records = await _dbContext.DisabilityTypes
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);

            return records
                .Select(CatalogPersistenceMapper.ToDomainDisabilityType)
                .ToList();
        }

        public async Task<IReadOnlyList<DisabilityType>> GetActiveAsync(
            CancellationToken cancellationToken = default)
        {
            var activeStatus = CatalogStatus.Active.ToString();

            var records = await _dbContext.DisabilityTypes
                .AsNoTracking()
                .Where(x => x.Status == activeStatus)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);

            return records
                .Select(CatalogPersistenceMapper.ToDomainDisabilityType)
                .ToList();
        }

        public async Task AddAsync(
            DisabilityType disabilityType,
            CancellationToken cancellationToken = default)
        {
            var record = CatalogPersistenceMapper.ToRecord(disabilityType);

            await _dbContext.DisabilityTypes.AddAsync(record, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            DisabilityType disabilityType,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.DisabilityTypes
                .FirstOrDefaultAsync(x => x.Id == disabilityType.Id, cancellationToken);

            if (record is null)
            {
                throw new InvalidOperationException("Không tìm thấy DisabilityType để cập nhật.");
            }

            CatalogPersistenceMapper.UpdateRecord(record, disabilityType);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
