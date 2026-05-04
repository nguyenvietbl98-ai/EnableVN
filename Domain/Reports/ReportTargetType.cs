using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Reports
{
    public enum ReportTargetType
    {
        JobPost = 1,          // Báo cáo tin tuyển dụng.
        EmployerProfile = 2,  // Báo cáo doanh nghiệp.
        CandidateProfile = 3  // Báo cáo ứng viên nếu cần.
    }
}
