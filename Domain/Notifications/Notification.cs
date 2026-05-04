using Domain.Common;
using System;

namespace Domain.Notifications
{
    public sealed class Notification : Entity<Guid>
    {
        public Guid UserId { get; private set; }

        public string Title { get; private set; } = string.Empty;

        public string Message { get; private set; } = string.Empty;

        public NotificationType Type { get; private set; }

        public NotificationStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? ReadAt { get; private set; }

        private Notification(Guid id) : base(id) { }

        public static Notification Create(
            Guid userId,
            string title,
            string message,
            NotificationType type)
        {
            if (userId == Guid.Empty)
                throw new DomainException("User nhận thông báo không hợp lệ.");

            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Tiêu đề thông báo không được để trống.");

            if (string.IsNullOrWhiteSpace(message))
                throw new DomainException("Nội dung thông báo không được để trống.");

            return new Notification(Guid.NewGuid())
            {
                UserId = userId,
                Title = title.Trim(),
                Message = message.Trim(),
                Type = type,
                Status = NotificationStatus.Unread,
                CreatedAt = DateTime.UtcNow,
                ReadAt = null
            };
        }

        public static Notification Restore(
            Guid id,
            Guid userId,
            string title,
            string message,
            NotificationType type,
            NotificationStatus status,
            DateTime createdAt,
            DateTime? readAt)
        {
            return new Notification(id)
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                Status = status,
                CreatedAt = createdAt,
                ReadAt = readAt
            };
        }

        public void MarkAsRead()
        {
            if (Status == NotificationStatus.Read)
                return;

            Status = NotificationStatus.Read;
            ReadAt = DateTime.UtcNow;
        }
    }
}
