using Ports.Models.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Inbound
{
    public interface ICompanyReviewUseCase
    {
        Task<Guid> CreateAsync(
            CreateCompanyReviewCommand command,
            CancellationToken cancellationToken = default
        );
        // Candidate tạo đánh giá doanh nghiệp.

        Task<IReadOnlyList<CompanyReviewResult>> GetByEmployerIdAsync(
            Guid employerId,
            CancellationToken cancellationToken = default
        );
        // Xem danh sách đánh giá của một doanh nghiệp.
    }
}
