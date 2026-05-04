using Ports.Models.Candidates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Inbound
{
    // UseCase cho Employer chủ động tìm ứng viên.
    public interface IEmployerCandidateSearchUseCase
    {
        Task<IReadOnlyList<CandidateProfileResult>> SearchAsync(
            SearchPublicCandidatesQuery query,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Employer xem một hồ sơ công khai theo Id (dùng cho trang chi tiết / AI gợi ý).
        /// </summary>
        Task<CandidateProfileResult?> GetPublicProfileByIdAsync(
            Guid candidateProfileId,
            CancellationToken cancellationToken = default
        );
    }
}
