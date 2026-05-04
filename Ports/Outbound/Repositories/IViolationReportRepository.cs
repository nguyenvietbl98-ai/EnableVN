using Domain.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Outbound.Repositories
{
    public interface IViolationReportRepository
    {
        Task AddAsync(
            ViolationReport report,
            CancellationToken cancellationToken = default
        );

        Task UpdateAsync(
            ViolationReport report,
            CancellationToken cancellationToken = default
        );

        Task<ViolationReport?> GetByIdAsync(
            Guid reportId,
            CancellationToken cancellationToken = default
        );

        Task<IReadOnlyList<ViolationReport>> GetByStatusAsync(
            ReportStatus status,
            CancellationToken cancellationToken = default
        );
    }
}

