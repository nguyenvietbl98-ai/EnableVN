using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Employers
{
    /// <summary>
    /// Command cập nhật hồ sơ doanh nghiệp.
    /// </summary>
    public sealed class UpdateEmployerProfileCommand
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
