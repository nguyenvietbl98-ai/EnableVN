using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Applications
{
    /// <summary>
    /// Command dùng khi Candidate nộp hồ sơ vào một job.
    /// CandidateId không nhận từ client, nên lấy từ current user.
    /// </summary>
    public sealed class SubmitJobApplicationCommand
    {
        public Guid JobId { get; init; }

        public string? CoverLetter { get; init; }

        public string? CvUrl { get; init; }
    }
}
