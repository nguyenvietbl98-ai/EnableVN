using Application.Common;
using Application.Mappers;
using Ports.Inbound;
using Ports.Models.Notifications;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases
{
    // UseCase này xử lý hành vi đọc thông báo của user.
    public sealed class NotificationUseCase : INotificationUseCase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUser;

        public NotificationUseCase(
            INotificationRepository notificationRepository,
            ICurrentUserService currentUser)
        {
            _notificationRepository = notificationRepository;
            _currentUser = currentUser;
        }

        public async Task<IReadOnlyList<NotificationResult>> GetMyNotificationsAsync(
            CancellationToken cancellationToken = default)
        {
            var userId = AuthorizationGuard.RequireAuthenticatedUser(_currentUser);
            // Chỉ user đăng nhập mới được xem thông báo.

            var notifications = await _notificationRepository.GetByUserIdAsync(
                userId,
                cancellationToken
            );

            return notifications
                .Select(NotificationMapper.ToResult)
                .ToList();
        }

        public async Task<int> CountMyUnreadAsync(
            CancellationToken cancellationToken = default)
        {
            var userId = AuthorizationGuard.RequireAuthenticatedUser(_currentUser);
            // Lấy user hiện tại từ Session/JWT thông qua ICurrentUserService.

            return await _notificationRepository.CountUnreadAsync(
                userId,
                cancellationToken
            );
        }

        public async Task MarkAsReadAsync(
            Guid notificationId,
            CancellationToken cancellationToken = default)
        {
            var userId = AuthorizationGuard.RequireAuthenticatedUser(_currentUser);

            var notification = await _notificationRepository.GetByIdAsync(
                notificationId,
                cancellationToken
            );

            if (notification is null)
                throw new UseCaseException("Không tìm thấy thông báo.");

            if (notification.UserId != userId)
                throw new UseCaseException("Bạn không có quyền đọc thông báo này.");

            notification.MarkAsRead();

            await _notificationRepository.UpdateAsync(
                notification,
                cancellationToken
            );
        }
    }

}
