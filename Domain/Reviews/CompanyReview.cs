using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Reviews
{
    // Review là đánh giá của Candidate dành cho Employer.
    public sealed class CompanyReview : Entity<Guid>
    {
        public Guid EmployerId { get; private set; } // Id EmployerProfile được đánh giá.

        public Guid CandidateId { get; private set; } // Id CandidateProfile tạo đánh giá.

        public int Rating { get; private set; } // Điểm từ 1 đến 5.

        public string Comment { get; private set; } = string.Empty; // Nội dung đánh giá.

        public DateTime CreatedAt { get; private set; } // Ngày tạo.

        private CompanyReview(Guid id) : base(id) { }

        public static CompanyReview Create(
            Guid employerId,
            Guid candidateId,
            int rating,
            string comment)
        {
            if (employerId == Guid.Empty)
                throw new DomainException("Doanh nghiệp không hợp lệ.");

            if (candidateId == Guid.Empty)
                throw new DomainException("Ứng viên không hợp lệ.");

            if (rating < 1 || rating > 5)
                throw new DomainException("Điểm đánh giá phải từ 1 đến 5.");

            if (string.IsNullOrWhiteSpace(comment))
                throw new DomainException("Nội dung đánh giá không được để trống.");

            return new CompanyReview(Guid.NewGuid())
            {
                EmployerId = employerId,
                CandidateId = candidateId,
                Rating = rating,
                Comment = comment.Trim(),
                CreatedAt = DateTime.UtcNow
            };
        }

        public static CompanyReview Restore(
            Guid id,
            Guid employerId,
            Guid candidateId,
            int rating,
            string comment,
            DateTime createdAt)
        {
            return new CompanyReview(id)
            {
                EmployerId = employerId,
                CandidateId = candidateId,
                Rating = rating,
                Comment = comment,
                CreatedAt = createdAt
            };
        }
    }
}
