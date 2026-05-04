using Domain.Applications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Applications
{
    /// <summary>
    /// Result trả về khi xem hồ sơ ứng tuyển.
    /// </summary>
    public sealed class JobApplicationResult
    {
        public Guid Id { get; init; }

        public Guid JobId { get; init; }

        public Guid CandidateId { get; init; }

        public string? CoverLetter { get; init; }

        public string? CvUrl { get; init; }

        public ApplicationStatus Status { get; init; }

        public DateTime SubmittedAt { get; init; }

        /// <summary>
        /// Cập nhật gần nhất trong lịch sử trạng thái (ghi chú kèm theo).
        /// </summary>
        public string? LatestHistoryNote { get; init; }

        /// <summary>
        /// Trạng thái gắn với mục lịch sử gần nhất.
        /// </summary>
        public ApplicationStatus? LatestHistoryStatus { get; init; }

        /// <summary>
        /// Thời điểm cập nhật gần nhất trong lịch sử.
        /// </summary>
        public DateTime? LatestHistoryAt { get; init; }

        /// <summary>
        /// Ghi chú gần nhất do nhà tuyển dụng nhập (bỏ qua dòng hệ thống tự tạo).
        /// </summary>
        public string? EmployerFeedbackNote { get; init; }

        public DateTime? EmployerFeedbackAt { get; init; }
    }
}
