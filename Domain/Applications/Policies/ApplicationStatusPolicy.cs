using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Applications.Policies
{
    public static class ApplicationStatusPolicy
    {
        public static void EnsureCanChangeStatus(
            ApplicationStatus currentStatus,
            ApplicationStatus newStatus
        )
        {
            if (currentStatus == ApplicationStatus.Withdrawn)
                throw new DomainException("Không thể đổi trạng thái hồ sơ đã được ứng viên rút.");

            if (currentStatus == ApplicationStatus.Rejected)
                throw new DomainException("Không thể đổi trạng thái hồ sơ đã bị từ chối.");

            if (currentStatus == ApplicationStatus.Accepted)
                throw new DomainException("Không thể đổi trạng thái hồ sơ đã được chấp nhận.");

            if (currentStatus == newStatus)
                throw new DomainException("Trạng thái mới trùng với trạng thái hiện tại.");
        }
    }
}
