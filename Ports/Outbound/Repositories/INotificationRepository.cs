using Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Outbound.Repositories
{
    // Interface này là Outbound Port.
    // Application chỉ biết hợp đồng này, không biết SQLite hay InMemory.
    public interface INotificationRepository
    {
        Task AddAsync(
            Notification notification,
            CancellationToken cancellationToken = default
        ); // Lưu thông báo mới.

        Task<Notification?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        ); // Lấy thông báo theo Id.

        Task<IReadOnlyList<Notification>> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        ); // Lấy danh sách thông báo của user hiện tại.

        Task<int> CountUnreadAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        ); // Đếm số thông báo chưa đọc.

        Task UpdateAsync(
            Notification notification,
            CancellationToken cancellationToken = default
        ); // Cập nhật trạng thái đã đọc.
    }
}
