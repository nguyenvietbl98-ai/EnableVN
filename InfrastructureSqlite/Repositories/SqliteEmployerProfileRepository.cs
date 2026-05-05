using Domain.Employers;
using InfrastructureSqlite.Mappers;
using InfrastructureSqlite.Persistence;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureSqlite.Repositories
{
    public sealed class SqliteEmployerProfileRepository : IEmployerProfileRepository
    {
        private readonly EnableVnDbContext _dbContext;

        public SqliteEmployerProfileRepository(EnableVnDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EmployerProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.EmployerProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return record is null
                ? null
                : EmployerProfilePersistenceMapper.ToDomain(record);
        }

        public async Task<EmployerProfile?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.EmployerProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            return record is null
                ? null
                : EmployerProfilePersistenceMapper.ToDomain(record);
        }

        public async Task<IReadOnlyList<EmployerProfile>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var rows = await _dbContext.EmployerProfiles
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return rows.Select(EmployerProfilePersistenceMapper.ToDomain).ToList();
        }

        public async Task<bool> ExistsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.EmployerProfiles
                .AnyAsync(x => x.UserId == userId, cancellationToken);
        }

        public async Task AddAsync(
            EmployerProfile profile,
            CancellationToken cancellationToken = default)
        {
            var record = EmployerProfilePersistenceMapper.ToRecord(profile);

            await _dbContext.EmployerProfiles.AddAsync(record, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            EmployerProfile profile,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.EmployerProfiles
                .FirstOrDefaultAsync(x => x.Id == profile.Id, cancellationToken);

            if (record is null)
            {
                throw new InvalidOperationException("Không tìm thấy EmployerProfile để cập nhật.");
            }

            EmployerProfilePersistenceMapper.UpdateRecord(record, profile);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
