using System;
using Domain.Reports;

namespace Ports.Models.Reports
{
    public sealed class ViolationReportResult
    {
        public Guid Id { get; init; }

        public Guid ReporterUserId { get; init; }

        public ReportTargetType TargetType { get; init; }

        public Guid TargetId { get; init; }

        public string Reason { get; init; } = string.Empty;

        public ReportStatus Status { get; init; }

        public string? AdminNote { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime? HandledAt { get; init; }
    }
}

