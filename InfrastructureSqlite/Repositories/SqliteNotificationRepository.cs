using Domain.Notifications;
using InfrastructureSqlite.Mappers;
using InfrastructureSqlite.Persistence;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureSqlite.Repositories
{
    // Repository này implement Outbound Port bằng SQLite.
    public sealed class SqliteNotificationRepository : INotificationRepository
    {
        private readonly EnableVnDbContext _dbContext;

        public SqliteNotificationRepository(EnableVnDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(
            Notification notification,
            CancellationToken cancellationToken = default)
        {
            var record = NotificationPersistenceMapper.ToRecord(notification);
            // Chuyển Domain Entity sang EF Record.

            _dbContext.Notifications.Add(record);

            await _dbContext.SaveChangesAsync(cancellationToken);
            // Lưu ngay vì repository hiện tại của dự án đang theo kiểu SaveChanges trong từng method.
        }

        public async Task<Notification?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.Notifications
                .AsNoTracking() // Query đọc không cần tracking.
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return record is null
                ? null
                : NotificationPersistenceMapper.ToDomain(record);
        }

        public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var records = await _dbContext.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt) // Thông báo mới nhất lên đầu.
                .ToListAsync(cancellationToken);

            return records
                .Select(NotificationPersistenceMapper.ToDomain)
                .ToList();
        }

        public async Task<int> CountUnreadAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Notifications
                .CountAsync(
                    x => x.UserId == userId && x.Status == NotificationStatus.Unread.ToString(),
                    cancellationToken
                );
        }

        public async Task UpdateAsync(
            Notification notification,
            CancellationToken cancellationToken = default)
        {
            var record = await _dbContext.Notifications
                .FirstOrDefaultAsync(x => x.Id == notification.Id, cancellationToken);

            if (record is null)
                return; // Không throw ở repository; UseCase đã kiểm tra nghiệp vụ.

            NotificationPersistenceMapper.UpdateRecord(record, notification);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
