using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Candidates
{
    /// <summary>
    /// Result trả về khi xem hồ sơ ứng viên.
    /// 
    /// Lưu ý:
    /// Nếu viewer là Employer và ứng viên không cho xem thông tin khuyết tật,
    /// Application nên không map DisabilityDescription ra response.
    /// </summary>
    public sealed class CandidateProfileResult
    {
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public string FullName { get; init; } = string.Empty;

        public string? Bio { get; init; }

        public string? CvUrl { get; init; }

        public Guid? DisabilityTypeId { get; init; }

        public string? DisabilityDescription { get; init; }

        public bool IsDisabilityInfoVisibleToEmployer { get; init; }

        public bool IsPublicProfile { get; init; }
    }
}
