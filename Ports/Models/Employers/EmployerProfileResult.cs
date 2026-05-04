using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Employers
{
    /// <summary>
    /// Result trả về cho Presentation khi xem hồ sơ doanh nghiệp.
    /// Không trả trực tiếp Domain Entity ra ngoài.
    /// </summary>
    public sealed class EmployerProfileResult
    {
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

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
