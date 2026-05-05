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

    public string? LogoUrl { get; init; }

    public string? ContactEmail { get; init; }

    public string? PhoneNumber { get; init; }

    public string? Address { get; init; }

    public string? CompanySize { get; init; }

    public string? Industry { get; init; }

    public string? TaxCode { get; init; }

    public string? RecruiterContactName { get; init; }

    public string? RecruiterContactTitle { get; init; }

        public string? Description { get; init; }

    public string? Benefits { get; init; }

    public string? Culture { get; init; }

        public string? WebsiteUrl { get; init; }

        public bool HasWheelchairAccess { get; init; }

        public bool HasAccessibleRestroom { get; init; }

        public bool SupportsFlexibleWorkingTime { get; init; }

        public bool SupportsRemoteWork { get; init; }

        public bool ProvidesAssistiveDevices { get; init; }
    }
}
