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
            ApplicationStatus newStatus,
            string? note
        )
        {
            if (currentStatus == ApplicationStatus.Withdrawn)
                throw new DomainException("Không thể đổi trạng thái hồ sơ đã được ứng viên rút.");

            if (currentStatus == ApplicationStatus.Rejected)
                throw new DomainException("Không thể đổi trạng thái hồ sơ đã bị từ chối.");

            if (currentStatus == ApplicationStatus.Accepted)
                throw new DomainException("Không thể đổi trạng thái hồ sơ đã được chấp nhận.");

            if (currentStatus == newStatus)
            {
                if (string.IsNullOrWhiteSpace(note))
                    throw new DomainException("Chọn trạng thái khác hoặc nhập nội dung phản hồi cho ứng viên (ghi chú).");

                return;
            }
        }
    }
}
