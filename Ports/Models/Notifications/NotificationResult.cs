using System;
using Domain.Notifications;

namespace Ports.Models.Notifications
{
    public sealed class NotificationResult
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = string.Empty;

        public string Message { get; init; } = string.Empty;

        public NotificationType Type { get; init; }

        public NotificationStatus Status { get; init; }

        public DateTimeOffset CreatedAt { get; init; }

        public DateTimeOffset? ReadAt { get; init; }
    }
}
