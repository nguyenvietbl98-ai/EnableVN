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
    public sealed class SqliteAssistiveDeviceRepository : IAssistiveDeviceRepository
    {
        private readonly EnableVnDbContext _dbContext;

        public SqliteAssistiveDeviceRepository(EnableVnDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AssistiveDevice?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.AssistiveDevices
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return record is null
                ? null
                : CatalogPersistenceMapper.ToDomainAssistiveDevice(record);
        }

        public async Task<IReadOnlyList<AssistiveDevice>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var records = await _dbContext.AssistiveDevices
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);

            return records
                .Select(CatalogPersistenceMapper.ToDomainAssistiveDevice)
                .ToList();
        }

        public async Task<IReadOnlyList<AssistiveDevice>> GetActiveAsync(
            CancellationToken cancellationToken = default)
        {
            var activeStatus = CatalogStatus.Active.ToString();

            var records = await _dbContext.AssistiveDevices
                .AsNoTracking()
                .Where(x => x.Status == activeStatus)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);

            return records
                .Select(CatalogPersistenceMapper.ToDomainAssistiveDevice)
                .ToList();
        }

        public async Task AddAsync(
            AssistiveDevice assistiveDevice,
            CancellationToken cancellationToken = default)
        {
            var record = CatalogPersistenceMapper.ToRecord(assistiveDevice);

            await _dbContext.AssistiveDevices.AddAsync(record, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            AssistiveDevice assistiveDevice,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.AssistiveDevices
                .FirstOrDefaultAsync(x => x.Id == assistiveDevice.Id, cancellationToken);

            if (record is null)
            {
                throw new InvalidOperationException("Không tìm thấy AssistiveDevice để cập nhật.");
            }

            CatalogPersistenceMapper.UpdateRecord(record, assistiveDevice);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
