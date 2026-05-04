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
                Description = profile.Description,
                WebsiteUrl = profile.WebsiteUrl,
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
                record.Description,
                record.WebsiteUrl,
                workplaceInfo
            );
        }

        public static void UpdateRecord(EmployerProfileRecord record, EmployerProfile profile)
        {
            record.CompanyName = profile.CompanyName.Value;
            record.Description = profile.Description;
            record.WebsiteUrl = profile.WebsiteUrl;
            record.HasWheelchairAccess = profile.WorkplaceInfo.HasWheelchairAccess;
            record.HasAccessibleRestroom = profile.WorkplaceInfo.HasAccessibleRestroom;
            record.SupportsFlexibleWorkingTime = profile.WorkplaceInfo.SupportsFlexibleWorkingTime;
            record.SupportsRemoteWork = profile.WorkplaceInfo.SupportsRemoteWork;
            record.ProvidesAssistiveDevices = profile.WorkplaceInfo.ProvidesAssistiveDevices;
        }
    }
}
