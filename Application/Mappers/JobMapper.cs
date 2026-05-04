using Domain.Jobs;
using Ports.Models.Jobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Mappers
{
    /// <summary>
    /// Mapper cho JobPost.
    /// </summary>
    public static class JobMapper
    {
        public static JobResult ToResult(JobPost job, Guid? employerUserId = null)
        {
            return new JobResult
            {
                Id = job.Id,
                EmployerId = job.EmployerId,
                EmployerUserId = employerUserId,

                Title = job.Title.Value,
                Description = job.Description,
                Requirement = job.Requirement,

                WorkMode = job.WorkMode,

                MinSalary = job.SalaryRange.MinSalary,
                MaxSalary = job.SalaryRange.MaxSalary,

                SupportsWheelchairAccess = job.AccessibilityInfo.SupportsWheelchairAccess,
                SupportsRemoteWork = job.AccessibilityInfo.SupportsRemoteWork,
                SupportsFlexibleTime = job.AccessibilityInfo.SupportsFlexibleTime,
                ProvidesAssistiveDevices = job.AccessibilityInfo.ProvidesAssistiveDevices,
                AdditionalSupportDescription = job.AccessibilityInfo.AdditionalSupportDescription,

                Status = job.Status,
                CreatedAt = job.CreatedAt,
                PublishedAt = job.PublishedAt,
                ClosedAt = job.ClosedAt
            };
        }
    }
}
