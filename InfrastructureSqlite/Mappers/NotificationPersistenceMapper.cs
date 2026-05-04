using Domain.Notifications;
using InfrastructureSqlite.PersistenceModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureSqlite.Mappers
{
    // Mapper chuyển giữa Domain Notification và EF Record.
    public static class NotificationPersistenceMapper
    {
        public static NotificationRecord ToRecord(Notification notification)
        {
            return new NotificationRecord
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(), // Enum lưu dạng string cho dễ đọc DB.
                Status = notification.Status.ToString(),
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt
            };
        }

        public static Notification ToDomain(NotificationRecord record)
        {
            return Notification.Restore(
                record.Id,
                record.UserId,
                record.Title,
                record.Message,
                Enum.Parse<NotificationType>(record.Type),
                Enum.Parse<NotificationStatus>(record.Status),
                record.CreatedAt,
                record.ReadAt
            );
        }

        public static void UpdateRecord(
            NotificationRecord record,
            Notification notification)
        {
            record.Title = notification.Title;
            record.Message = notification.Message;
            record.Type = notification.Type.ToString();
            record.Status = notification.Status.ToString();
            record.CreatedAt = notification.CreatedAt;
            record.ReadAt = notification.ReadAt;
        }
    }
}
