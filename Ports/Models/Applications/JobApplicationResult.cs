using Domain.Applications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Applications
{
    /// <summary>
    /// Result trả về khi xem hồ sơ ứng tuyển.
    /// </summary>
    public sealed class JobApplicationResult
    {
        public Guid Id { get; init; }

        public Guid JobId { get; init; }

        public Guid CandidateId { get; init; }

        public string? CoverLetter { get; init; }

        public string? CvUrl { get; init; }

        public ApplicationStatus Status { get; init; }

        public DateTime SubmittedAt { get; init; }
    }
}
