using Domain.Jobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Jobs
{
    /// <summary>
    /// Result trả về khi xem chi tiết hoặc danh sách job.
    /// Không expose trực tiếp Domain Entity ra Presentation.
    /// </summary>
    public sealed class JobResult
    {
        public Guid Id { get; init; }

        public Guid EmployerId { get; init; }

        /// <summary>
        /// UserId của tài khoản nhà tuyển dụng sở hữu tin (để ẩn nút ứng tuyển khi trùng người đăng).
        /// </summary>
        public Guid? EmployerUserId { get; init; }

        public string Title { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public string Requirement { get; init; } = string.Empty;

        public WorkMode WorkMode { get; init; }

        public decimal? MinSalary { get; init; }

        public decimal? MaxSalary { get; init; }

        public bool SupportsWheelchairAccess { get; init; }

        public bool SupportsRemoteWork { get; init; }

        public bool SupportsFlexibleTime { get; init; }

        public bool ProvidesAssistiveDevices { get; init; }

        public string? AdditionalSupportDescription { get; init; }

        public JobStatus Status { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime? PublishedAt { get; init; }

        public DateTime? ClosedAt { get; init; }
    }
}
