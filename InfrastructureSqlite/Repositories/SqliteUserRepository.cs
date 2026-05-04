using Domain.Users;
using InfrastructureSqlite.Mappers;
using InfrastructureSqlite.Persistence;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureSqlite.Repositories
{
    public sealed class SqliteUserRepository : IUserRepository
    {
        private readonly EnableVnDbContext _dbContext;

        public SqliteUserRepository(EnableVnDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return record is null
                ? null
                : UserPersistenceMapper.ToDomain(record);
        }

        public async Task<User?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            var record = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Email.ToLower() == normalizedEmail,
                    cancellationToken);

            return record is null
                ? null
                : UserPersistenceMapper.ToDomain(record);
        }

        public async Task<bool> ExistsByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            return await _dbContext.Users
                .AnyAsync(
                    x => x.Email.ToLower() == normalizedEmail,
                    cancellationToken);
        }

        public async Task AddAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            var record = UserPersistenceMapper.ToRecord(user);

            await _dbContext.Users.AddAsync(record, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == user.Id, cancellationToken);

            if (record is null)
            {
                throw new InvalidOperationException("Không tìm thấy User để cập nhật.");
            }

            UserPersistenceMapper.UpdateRecord(record, user);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
