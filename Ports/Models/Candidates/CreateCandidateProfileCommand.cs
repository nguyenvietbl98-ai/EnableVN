using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Candidates
{
    /// <summary>
    /// Command tạo hồ sơ ứng viên.
    /// UserId sẽ lấy từ ICurrentUserService, không nhận từ client.
    /// </summary>
    public sealed class CreateCandidateProfileCommand
    {
        public string FullName { get; init; } = string.Empty;

        public string? Bio { get; init; }

        public string? CvUrl { get; init; }
    }
}
