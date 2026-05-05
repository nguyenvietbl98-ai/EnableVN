using Domain.Employers;
using Ports.Models.Employers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Mappers
{
    /// <summary>
    /// Mapper cho EmployerProfile.
    /// 
    /// Không trả Domain Entity trực tiếp ra Presentation.
    /// Application map Domain sang Result model.
    /// </summary>
    public static class EmployerProfileMapper
    {
        public static EmployerProfileResult ToResult(EmployerProfile profile)
        {
            return new EmployerProfileResult
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
    }
}
