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
                Description = profile.Description,
                WebsiteUrl = profile.WebsiteUrl,

                HasWheelchairAccess = profile.WorkplaceInfo.HasWheelchairAccess,
                HasAccessibleRestroom = profile.WorkplaceInfo.HasAccessibleRestroom,
                SupportsFlexibleWorkingTime = profile.WorkplaceInfo.SupportsFlexibleWorkingTime,
                SupportsRemoteWork = profile.WorkplaceInfo.SupportsRemoteWork,
                ProvidesAssistiveDevices = profile.WorkplaceInfo.ProvidesAssistiveDevices
            };
        }
    }
}
