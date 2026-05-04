using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Candidates.Events
{
    public sealed class CandidateProfileCreatedEvent : IDomainEvent
    {
        public Guid CandidateProfileId { get; }
        public Guid UserId { get; }
        public DateTime OccurredOn { get; }

        public CandidateProfileCreatedEvent(Guid candidateProfileId, Guid userId)
        {
            CandidateProfileId = candidateProfileId;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
