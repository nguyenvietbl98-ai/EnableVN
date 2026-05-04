using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Users.Events
{
    public sealed class UserRegisteredEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public UserRole Role { get; }
        public DateTime OccurredOn { get; }

        public UserRegisteredEvent(Guid userId, UserRole role)
        {
            UserId = userId;
            Role = role;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
