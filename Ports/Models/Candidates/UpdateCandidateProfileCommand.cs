using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Candidates
{
    /// <summary>
    /// Command cập nhật thông tin cơ bản của ứng viên.
    /// </summary>
    public sealed class UpdateCandidateProfileCommand
    {
        public string FullName { get; init; } = string.Empty;

        public string? Bio { get; init; }

        public string? CvUrl { get; init; }
    }
}
