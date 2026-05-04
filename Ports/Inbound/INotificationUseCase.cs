using Ports.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Inbound
{
    // Interface này để Presentation gọi vào Application.
    public interface INotificationUseCase
    {
        Task<IReadOnlyList<NotificationResult>> GetMyNotificationsAsync(
            CancellationToken cancellationToken = default
        ); // User xem thông báo của chính mình.

        Task<int> CountMyUnreadAsync(
            CancellationToken cancellationToken = default
        ); // Hiển thị badge số lượng thông báo chưa đọc.

        Task MarkAsReadAsync(
            Guid notificationId,
            CancellationToken cancellationToken = default
        ); // Đánh dấu một thông báo là đã đọc.
    }
}
