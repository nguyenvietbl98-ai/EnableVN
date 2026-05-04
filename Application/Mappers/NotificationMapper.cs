using Domain.Notifications;
using Ports.Models.Notifications;

namespace Application.Mappers
{
    // Mapper giúp tách Domain Entity khỏi DTO trả ra UI.
    public static class NotificationMapper
    {
        public static NotificationResult ToResult(Notification notification)
        {
            return new NotificationResult
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                Status = notification.Status,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt
            };
        }
    }
}
