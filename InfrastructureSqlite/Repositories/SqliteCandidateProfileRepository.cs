using Domain.Candidates;
using InfrastructureSqlite.Mappers;
using InfrastructureSqlite.Persistence;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureSqlite.Repositories
{
    public sealed class SqliteCandidateProfileRepository : ICandidateProfileRepository
    {
        private readonly EnableVnDbContext _dbContext;

        public SqliteCandidateProfileRepository(EnableVnDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CandidateProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.CandidateProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return record is null
                ? null
                : CandidateProfilePersistenceMapper.ToDomain(record);
        }

        public async Task<CandidateProfile?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.CandidateProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            return record is null
                ? null
                : CandidateProfilePersistenceMapper.ToDomain(record);
        }

        public async Task<bool> ExistsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.CandidateProfiles
                .AnyAsync(x => x.UserId == userId, cancellationToken);
        }

        public async Task<IReadOnlyList<CandidateProfile>> GetPublicProfilesAsync(
            CancellationToken cancellationToken = default)
        {
            var records = await _dbContext.CandidateProfiles
                .AsNoTracking()
                .Where(x => x.IsPublicProfile)
                .ToListAsync(cancellationToken);

            return records
                .Select(CandidateProfilePersistenceMapper.ToDomain)
                .ToList();
        }

        public async Task AddAsync(
            CandidateProfile profile,
            CancellationToken cancellationToken = default)
        {
            var record = CandidateProfilePersistenceMapper.ToRecord(profile);

            await _dbContext.CandidateProfiles.AddAsync(record, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            CandidateProfile profile,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.CandidateProfiles
                .FirstOrDefaultAsync(x => x.Id == profile.Id, cancellationToken);

            if (record is null)
            {
                throw new InvalidOperationException("Không tìm thấy CandidateProfile để cập nhật.");
            }

            CandidateProfilePersistenceMapper.UpdateRecord(record, profile);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
