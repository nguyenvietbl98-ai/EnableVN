using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Reports
{
    public enum ReportStatus
    {
        Pending = 1,   // Chờ Admin xử lý.
        Resolved = 2,  // Đã xử lý.
        Rejected = 3   // Từ chối báo cáo.
    }
}
