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

        public string VerificationStatus { get; init; } = string.Empty;

        public DateTime? VerifiedAtUtc { get; init; }

        public string? VerificationNote { get; init; }

        public bool HasWheelchairAccess { get; init; }

        public bool HasAccessibleRestroom { get; init; }

        public bool SupportsFlexibleWorkingTime { get; init; }

        public bool SupportsRemoteWork { get; init; }

        public bool ProvidesAssistiveDevices { get; init; }
    }
}
