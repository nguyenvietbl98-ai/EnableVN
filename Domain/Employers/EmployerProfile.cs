using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Employers.Events;

namespace Domain.Employers
{
    public sealed class EmployerProfile : AggregateRoot<Guid>
    {
        public Guid UserId { get; private set; }
        public CompanyName CompanyName { get; private set; }
        public string? LogoUrl { get; private set; }
        public string? ContactEmail { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? Address { get; private set; }
        public string? CompanySize { get; private set; }
        public string? Industry { get; private set; }
        public string? TaxCode { get; private set; }
        public string? RecruiterContactName { get; private set; }
        public string? RecruiterContactTitle { get; private set; }
        public string? Description { get; private set; }
        public string? Benefits { get; private set; }
        public string? Culture { get; private set; }
        public string? WebsiteUrl { get; private set; }
        public EmployerVerificationStatus VerificationStatus { get; private set; }
        public DateTime? VerifiedAtUtc { get; private set; }
        public string? VerificationNote { get; private set; }
        public InclusiveWorkplaceInfo WorkplaceInfo { get; private set; }

        private EmployerProfile(
            Guid id,
            Guid userId,
            CompanyName companyName,
            string? logoUrl,
            string? contactEmail,
            string? phoneNumber,
            string? address,
            string? companySize,
            string? industry,
            string? taxCode,
            string? recruiterContactName,
            string? recruiterContactTitle,
            string? description,
            string? benefits,
            string? culture,
            string? websiteUrl,
            EmployerVerificationStatus verificationStatus,
            DateTime? verifiedAtUtc,
            string? verificationNote,
            InclusiveWorkplaceInfo workplaceInfo
        ) : base(id)
        {
            UserId = userId;
            CompanyName = companyName;
            LogoUrl = logoUrl;
            ContactEmail = contactEmail;
            PhoneNumber = phoneNumber;
            Address = address;
            CompanySize = companySize;
            Industry = industry;
            TaxCode = taxCode;
            RecruiterContactName = recruiterContactName;
            RecruiterContactTitle = recruiterContactTitle;
            Description = description;
            Benefits = benefits;
            Culture = culture;
            WebsiteUrl = websiteUrl;
            VerificationStatus = verificationStatus;
            VerifiedAtUtc = verifiedAtUtc;
            VerificationNote = verificationNote;
            WorkplaceInfo = workplaceInfo;
        }

        public static EmployerProfile Create(
            Guid userId,
            string companyName,
            string? description,
            string? websiteUrl,
            InclusiveWorkplaceInfo workplaceInfo
        )
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId của nhà tuyển dụng không hợp lệ.");

            var profile = new EmployerProfile(
                Guid.NewGuid(),
                userId,
                CompanyName.Create(companyName),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                description,
                null,
                null,
                websiteUrl,
                EmployerVerificationStatus.Pending,
                null,
                "Hồ sơ mới tạo, chờ admin kiểm duyệt.",
                workplaceInfo
            );

            profile.AddDomainEvent(
                new EmployerProfileCreatedEvent(profile.Id, userId)
            );

            return profile;
        }

        public void UpdateCompanyInfo(
            string companyName,
            string? logoUrl,
            string? contactEmail,
            string? phoneNumber,
            string? address,
            string? companySize,
            string? industry,
            string? taxCode,
            string? recruiterContactName,
            string? recruiterContactTitle,
            string? description,
            string? benefits,
            string? culture,
            string? websiteUrl
        )
        {
            CompanyName = CompanyName.Create(companyName);
            LogoUrl = logoUrl;
            ContactEmail = contactEmail;
            PhoneNumber = phoneNumber;
            Address = address;
            CompanySize = companySize;
            Industry = industry;
            TaxCode = taxCode;
            RecruiterContactName = recruiterContactName;
            RecruiterContactTitle = recruiterContactTitle;
            Description = description;
            Benefits = benefits;
            Culture = culture;
            WebsiteUrl = websiteUrl;
            VerificationStatus = EmployerVerificationStatus.Pending;
            VerifiedAtUtc = null;
            VerificationNote = "Hồ sơ đã thay đổi và cần admin duyệt lại trước khi đăng tin.";
        }

        public void UpdateInclusiveWorkplaceInfo(InclusiveWorkplaceInfo workplaceInfo)
        {
            WorkplaceInfo = workplaceInfo;
            VerificationStatus = EmployerVerificationStatus.Pending;
            VerifiedAtUtc = null;
            VerificationNote = "Hồ sơ đã thay đổi và cần admin duyệt lại trước khi đăng tin.";
        }

        public void ApproveByAdmin(string? note)
        {
            VerificationStatus = EmployerVerificationStatus.Approved;
            VerifiedAtUtc = DateTime.UtcNow;
            VerificationNote = string.IsNullOrWhiteSpace(note)
                ? "Hồ sơ đã được admin duyệt."
                : note.Trim();
        }

        public void RejectByAdmin(string? note)
        {
            VerificationStatus = EmployerVerificationStatus.Rejected;
            VerifiedAtUtc = DateTime.UtcNow;
            VerificationNote = string.IsNullOrWhiteSpace(note)
                ? "Hồ sơ chưa đạt yêu cầu, vui lòng bổ sung."
                : note.Trim();
        }

        public static EmployerProfile Restore(
            Guid id,
            Guid userId,
            string companyName,
            string? logoUrl,
            string? contactEmail,
            string? phoneNumber,
            string? address,
            string? companySize,
            string? industry,
            string? taxCode,
            string? recruiterContactName,
            string? recruiterContactTitle,
            string? description,
            string? benefits,
            string? culture,
            string? websiteUrl,
            EmployerVerificationStatus verificationStatus,
            DateTime? verifiedAtUtc,
            string? verificationNote,
            InclusiveWorkplaceInfo workplaceInfo
        )
        {
            if (id == Guid.Empty)
                throw new DomainException("EmployerProfileId không hợp lệ.");

            if (userId == Guid.Empty)
                throw new DomainException("UserId không hợp lệ.");

            return new EmployerProfile(
                id,
                userId,
                CompanyName.Create(companyName),
                logoUrl,
                contactEmail,
                phoneNumber,
                address,
                companySize,
                industry,
                taxCode,
                recruiterContactName,
                recruiterContactTitle,
                description,
                benefits,
                culture,
                websiteUrl,
                verificationStatus,
                verifiedAtUtc,
                verificationNote,
                workplaceInfo
            );
        }
    }
}
