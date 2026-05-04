using Application.Common;
using Domain.Reviews;
using Ports.Inbound;
using Ports.Models.Reviews;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases
{
    public sealed class CompanyReviewUseCase : ICompanyReviewUseCase
    {
        private readonly ICompanyReviewRepository _reviewRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IEmployerProfileRepository _employerProfileRepository;
        private readonly ICurrentUserService _currentUser;

        public CompanyReviewUseCase(
            ICompanyReviewRepository reviewRepository,
            ICandidateProfileRepository candidateProfileRepository,
            IJobApplicationRepository jobApplicationRepository,
            IJobRepository jobRepository,
            IEmployerProfileRepository employerProfileRepository,
            ICurrentUserService currentUser)
        {
            _reviewRepository = reviewRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _jobRepository = jobRepository;
            _employerProfileRepository = employerProfileRepository;
            _currentUser = currentUser;
        }

        public async Task<Guid> CreateAsync(
            CreateCompanyReviewCommand command,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = AuthorizationGuard.RequireCandidate(_currentUser);
            // Chỉ tài khoản Candidate mới được đánh giá doanh nghiệp.

            var candidateProfile = await _candidateProfileRepository.GetByUserIdAsync(
                currentUserId,
                cancellationToken
            );
            // Lấy hồ sơ Candidate từ user đang đăng nhập.

            if (candidateProfile is null)
                throw new UseCaseException("Bạn cần tạo hồ sơ ứng viên trước khi đánh giá doanh nghiệp.");

            var employerProfile = await _employerProfileRepository.GetByIdAsync(
                command.EmployerId,
                cancellationToken
            );
            // Kiểm tra doanh nghiệp được đánh giá có tồn tại không.

            if (employerProfile is null)
                throw new UseCaseException("Không tìm thấy doanh nghiệp cần đánh giá.");

            var alreadyReviewed = await _reviewRepository.ExistsByEmployerIdAndCandidateIdAsync(
                command.EmployerId,
                candidateProfile.Id,
                cancellationToken
            );
            // Kiểm tra ứng viên này đã từng đánh giá doanh nghiệp này chưa.

            if (alreadyReviewed)
                throw new UseCaseException("Bạn đã đánh giá doanh nghiệp này rồi.");

            var applications = await _jobApplicationRepository.GetByCandidateIdAsync(
                candidateProfile.Id,
                cancellationToken
            );
            // Lấy toàn bộ hồ sơ ứng tuyển của Candidate.

            var hasAppliedToThisEmployer = false;
            // Biến này dùng để kiểm tra Candidate có từng ứng tuyển vào Employer này không.

            foreach (var application in applications)
            {
                var job = await _jobRepository.GetByIdAsync(
                    application.JobId,
                    cancellationToken
                );
                // Từ hồ sơ ứng tuyển, lấy ra Job tương ứng.

                if (job is not null && job.EmployerId == command.EmployerId)
                {
                    hasAppliedToThisEmployer = true;
                    break;
                }
                // Nếu job đó thuộc Employer đang được đánh giá thì cho phép đánh giá.
            }

            if (!hasAppliedToThisEmployer)
                throw new UseCaseException("Bạn chỉ được đánh giá doanh nghiệp mà bạn từng ứng tuyển.");

            var review = CompanyReview.Create(
                command.EmployerId,
                candidateProfile.Id,
                command.Rating,
                command.Comment
            );
            // Sau khi qua hết các rule, mới tạo Domain Entity CompanyReview.

            await _reviewRepository.AddAsync(
                review,
                cancellationToken
            );
            // Lưu review vào database thông qua repository.

            return review.Id;
        }

        public async Task<IReadOnlyList<CompanyReviewResult>> GetByEmployerIdAsync(
            Guid employerId,
            CancellationToken cancellationToken = default)
        {
            var reviews = await _reviewRepository.GetByEmployerIdAsync(
                employerId,
                cancellationToken
            );
            // Ai cũng có thể xem review của doanh nghiệp.

            return reviews
                .Select(x => new CompanyReviewResult
                {
                    Id = x.Id,
                    EmployerId = x.EmployerId,
                    CandidateId = x.CandidateId,
                    Rating = x.Rating,
                    Comment = x.Comment,
                    CreatedAt = x.CreatedAt
                })
                .ToList();
        }
    }
}
