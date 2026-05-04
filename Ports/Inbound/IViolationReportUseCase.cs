using Ports.Models.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Inbound
{
    public interface IViolationReportUseCase
    {
        Task<Guid> CreateAsync(
            CreateViolationReportCommand command,
            CancellationToken cancellationToken = default
        );
        // User đăng nhập gửi báo cáo.

        Task<IReadOnlyList<ViolationReportResult>> GetPendingReportsAsync(
            CancellationToken cancellationToken = default
        );
        // Admin xem danh sách báo cáo đang chờ xử lý.

        Task ResolveAsync(
            HandleViolationReportCommand command,
            CancellationToken cancellationToken = default
        );
        // Admin xác nhận báo cáo là đúng và đã xử lý.

        Task RejectAsync(
            HandleViolationReportCommand command,
            CancellationToken cancellationToken = default
        );
        // Admin từ chối báo cáo.
    }
}
