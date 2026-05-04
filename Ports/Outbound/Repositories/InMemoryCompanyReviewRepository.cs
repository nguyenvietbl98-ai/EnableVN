using Domain.Reviews;
using Ports.Outbound.Repositories;

namespace InfrastructureInMemory.Repositories
{
    public class InMemoryCompanyReviewRepository : ICompanyReviewRepository
    {
        private static readonly List<CompanyReview> _reviews = new();

        public Task AddAsync(CompanyReview review, CancellationToken cancellationToken = default)
        {
            _reviews.Add(review);
            return Task.CompletedTask;
        }

        // Đã sửa tên hàm khớp với Interface: ExistsByEmployerIdAndCandidateIdAsync
        public Task<bool> ExistsByEmployerIdAndCandidateIdAsync(Guid employerId, Guid candidateId, CancellationToken cancellationToken = default)
        {
            var hasReviewed = _reviews.Any(r => r.EmployerId == employerId && r.CandidateId == candidateId);
            return Task.FromResult(hasReviewed);
        }

        // Đã sửa kiểu trả về thành IReadOnlyList<CompanyReview>
        public Task<IReadOnlyList<CompanyReview>> GetByEmployerIdAsync(Guid employerId, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<CompanyReview> results = _reviews.Where(r => r.EmployerId == employerId).ToList();
            return Task.FromResult(results);
        }
    }
}