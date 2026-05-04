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
    }
}
