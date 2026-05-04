using Domain.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Outbound.Repositories
{
    public interface ICompanyReviewRepository
    {
        Task AddAsync(
            CompanyReview review,
            CancellationToken cancellationToken = default
        ); // Thêm đánh giá mới.

        Task<IReadOnlyList<CompanyReview>> GetByEmployerIdAsync(
            Guid employerId,
            CancellationToken cancellationToken = default
        ); // Xem đánh giá của một doanh nghiệp.

        Task<bool> ExistsByEmployerIdAndCandidateIdAsync(
            Guid employerId,
            Guid candidateId,
            CancellationToken cancellationToken = default
        ); // Chặn một Candidate đánh giá trùng một Employer.
    }
}
