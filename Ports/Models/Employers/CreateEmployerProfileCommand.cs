using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Employers
{
    /// <summary>
    /// Command tạo hồ sơ doanh nghiệp.
    /// UserId không đặt trong command vì nên lấy từ ICurrentUserService.
    /// Như vậy client không thể giả mạo UserId.
    /// </summary>
    public sealed class CreateEmployerProfileCommand
    {
        public string CompanyName { get; init; } = string.Empty;

        public string? Description { get; init; }

        public string? WebsiteUrl { get; init; }

        public bool HasWheelchairAccess { get; init; }

        public bool HasAccessibleRestroom { get; init; }

        public bool SupportsFlexibleWorkingTime { get; init; }

        public bool SupportsRemoteWork { get; init; }

        public bool ProvidesAssistiveDevices { get; init; }
    }
}
