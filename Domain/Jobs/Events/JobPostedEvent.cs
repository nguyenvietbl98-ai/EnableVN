using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Jobs.Events
{
    public sealed class JobPostedEvent : IDomainEvent
    {
        public Guid JobId { get; }
        public Guid EmployerId { get; }
        public DateTime OccurredOn { get; }

        public JobPostedEvent(Guid jobId, Guid employerId)
        {
            JobId = jobId;
            EmployerId = employerId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
