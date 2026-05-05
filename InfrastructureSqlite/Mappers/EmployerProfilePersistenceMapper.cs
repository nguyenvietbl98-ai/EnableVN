using InfrastructureSqlite.PersistenceModels;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Employers;

namespace InfrastructureSqlite.Mappers
{
    public static class EmployerProfilePersistenceMapper
    {
        public static EmployerProfileRecord ToRecord(EmployerProfile profile)
        {
            return new EmployerProfileRecord
            {
                Id = profile.Id,
                UserId = profile.UserId,
                CompanyName = profile.CompanyName.Value,
                LogoUrl = profile.LogoUrl,
                ContactEmail = profile.ContactEmail,
                PhoneNumber = profile.PhoneNumber,
                Address = profile.Address,
                CompanySize = profile.CompanySize,
                Industry = profile.Industry,
                TaxCode = profile.TaxCode,
                RecruiterContactName = profile.RecruiterContactName,
                RecruiterContactTitle = profile.RecruiterContactTitle,
                Description = profile.Description,
                Benefits = profile.Benefits,
                Culture = profile.Culture,
                WebsiteUrl = profile.WebsiteUrl,
                VerificationStatus = profile.VerificationStatus.ToString(),
                VerifiedAtUtc = profile.VerifiedAtUtc,
                VerificationNote = profile.VerificationNote,
                HasWheelchairAccess = profile.WorkplaceInfo.HasWheelchairAccess,
                HasAccessibleRestroom = profile.WorkplaceInfo.HasAccessibleRestroom,
                SupportsFlexibleWorkingTime = profile.WorkplaceInfo.SupportsFlexibleWorkingTime,
                SupportsRemoteWork = profile.WorkplaceInfo.SupportsRemoteWork,
                ProvidesAssistiveDevices = profile.WorkplaceInfo.ProvidesAssistiveDevices
            };
        }

        public static EmployerProfile ToDomain(EmployerProfileRecord record)
        {
            var workplaceInfo = InclusiveWorkplaceInfo.Create(
                record.HasWheelchairAccess,
                record.HasAccessibleRestroom,
                record.SupportsFlexibleWorkingTime,
                record.SupportsRemoteWork,
                record.ProvidesAssistiveDevices
            );

            return EmployerProfile.Restore(
                record.Id,
                record.UserId,
                record.CompanyName,
                record.LogoUrl,
                record.ContactEmail,
                record.PhoneNumber,
                record.Address,
                record.CompanySize,
                record.Industry,
                record.TaxCode,
                record.RecruiterContactName,
                record.RecruiterContactTitle,
                record.Description,
                record.Benefits,
                record.Culture,
                record.WebsiteUrl,
                Enum.TryParse<EmployerVerificationStatus>(record.VerificationStatus, out var verificationStatus)
                    ? verificationStatus
                    : EmployerVerificationStatus.Pending,
                record.VerifiedAtUtc,
                record.VerificationNote,
                workplaceInfo
            );
        }

        public static void UpdateRecord(EmployerProfileRecord record, EmployerProfile profile)
        {
            record.CompanyName = profile.CompanyName.Value;
            record.LogoUrl = profile.LogoUrl;
            record.ContactEmail = profile.ContactEmail;
            record.PhoneNumber = profile.PhoneNumber;
            record.Address = profile.Address;
            record.CompanySize = profile.CompanySize;
            record.Industry = profile.Industry;
            record.TaxCode = profile.TaxCode;
            record.RecruiterContactName = profile.RecruiterContactName;
            record.RecruiterContactTitle = profile.RecruiterContactTitle;
            record.Description = profile.Description;
            record.Benefits = profile.Benefits;
            record.Culture = profile.Culture;
            record.WebsiteUrl = profile.WebsiteUrl;
            record.VerificationStatus = profile.VerificationStatus.ToString();
            record.VerifiedAtUtc = profile.VerifiedAtUtc;
            record.VerificationNote = profile.VerificationNote;
            record.HasWheelchairAccess = profile.WorkplaceInfo.HasWheelchairAccess;
            record.HasAccessibleRestroom = profile.WorkplaceInfo.HasAccessibleRestroom;
            record.SupportsFlexibleWorkingTime = profile.WorkplaceInfo.SupportsFlexibleWorkingTime;
            record.SupportsRemoteWork = profile.WorkplaceInfo.SupportsRemoteWork;
            record.ProvidesAssistiveDevices = profile.WorkplaceInfo.ProvidesAssistiveDevices;
        }
    }
}
