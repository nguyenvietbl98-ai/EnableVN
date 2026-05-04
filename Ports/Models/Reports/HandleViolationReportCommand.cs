using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Reports
{
    public sealed class HandleViolationReportCommand
    {
        public Guid ReportId { get; set; }
        // Id của báo cáo cần xử lý.

        public string? AdminNote { get; set; }
        // Ghi chú của Admin khi xử lý báo cáo.
    }
}
