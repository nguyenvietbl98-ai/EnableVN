using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Employers.Events
{
    public sealed class EmployerProfileCreatedEvent : IDomainEvent
    {
        public Guid EmployerProfileId { get; }
        public Guid UserId { get; }
        public DateTime OccurredOn { get; }

        public EmployerProfileCreatedEvent(Guid employerProfileId, Guid userId)
        {
            EmployerProfileId = employerProfileId;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
