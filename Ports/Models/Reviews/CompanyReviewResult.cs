using System;

namespace Ports.Models.Reviews
{
    public sealed class CompanyReviewResult
    {
        public Guid Id { get; init; }

        public Guid EmployerId { get; init; }

        public Guid CandidateId { get; init; }

        public int Rating { get; init; }

        public string Comment { get; init; } = string.Empty;

        public DateTime CreatedAt { get; init; }
    }
}

