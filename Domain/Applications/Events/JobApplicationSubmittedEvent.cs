using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Applications.Events
{
    public sealed class JobApplicationSubmittedEvent : IDomainEvent
    {
        public Guid ApplicationId { get; }
        public Guid JobId { get; }
        public Guid CandidateId { get; }
        public DateTime OccurredOn { get; }

        public JobApplicationSubmittedEvent(
            Guid applicationId,
            Guid jobId,
            Guid candidateId
        )
        {
            ApplicationId = applicationId;
            JobId = jobId;
            CandidateId = candidateId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
