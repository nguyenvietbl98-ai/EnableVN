using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Applications.Events
{
    public sealed class JobApplicationStatusChangedEvent : IDomainEvent
    {
        public Guid ApplicationId { get; }
        public Guid JobId { get; }
        public Guid CandidateId { get; }
        public ApplicationStatus OldStatus { get; }
        public ApplicationStatus NewStatus { get; }
        public DateTime OccurredOn { get; }

        public JobApplicationStatusChangedEvent(
            Guid applicationId,
            Guid jobId,
            Guid candidateId,
            ApplicationStatus oldStatus,
            ApplicationStatus newStatus
        )
        {
            ApplicationId = applicationId;
            JobId = jobId;
            CandidateId = candidateId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
