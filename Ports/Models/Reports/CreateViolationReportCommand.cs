using Domain.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Reports
{
    public sealed class CreateViolationReportCommand
    {
        public ReportTargetType TargetType { get; set; }
        // Loại đối tượng bị báo cáo: JobPost, EmployerProfile, CandidateProfile.

        public Guid TargetId { get; set; }
        // Id cụ thể của đối tượng bị báo cáo.

        public string Reason { get; set; } = string.Empty;
        // Lý do báo cáo.
    }
}
