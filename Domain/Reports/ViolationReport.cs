using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Reports
{
    public sealed class ViolationReport : Entity<Guid>
    {
        public Guid ReporterUserId { get; private set; } // User gửi báo cáo.

        public ReportTargetType TargetType { get; private set; } // Loại đối tượng bị báo cáo.

        public Guid TargetId { get; private set; } // Id của job/employer/candidate bị báo cáo.

        public string Reason { get; private set; } = string.Empty; // Lý do báo cáo.

        public ReportStatus Status { get; private set; } // Trạng thái xử lý.

        public string? AdminNote { get; private set; } // Ghi chú của Admin.

        public DateTime CreatedAt { get; private set; } // Ngày tạo.

        public DateTime? HandledAt { get; private set; } // Ngày xử lý.

        private ViolationReport(Guid id) : base(id) { }

        public static ViolationReport Create(
            Guid reporterUserId,
            ReportTargetType targetType,
            Guid targetId,
            string reason)
        {
            if (reporterUserId == Guid.Empty)
                throw new DomainException("Người báo cáo không hợp lệ.");

            if (targetId == Guid.Empty)
                throw new DomainException("Đối tượng bị báo cáo không hợp lệ.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Lý do báo cáo không được để trống.");

            return new ViolationReport(Guid.NewGuid())
            {
                ReporterUserId = reporterUserId,
                TargetType = targetType,
                TargetId = targetId,
                Reason = reason.Trim(),
                Status = ReportStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Resolve(string? adminNote)
        {
            if (Status != ReportStatus.Pending)
                throw new DomainException("Báo cáo này đã được xử lý.");

            Status = ReportStatus.Resolved;
            AdminNote = adminNote;
            HandledAt = DateTime.UtcNow;
        }

        public void Reject(string? adminNote)
        {
            if (Status != ReportStatus.Pending)
                throw new DomainException("Báo cáo này đã được xử lý.");

            Status = ReportStatus.Rejected;
            AdminNote = adminNote;
            HandledAt = DateTime.UtcNow;
        }
    }
}
