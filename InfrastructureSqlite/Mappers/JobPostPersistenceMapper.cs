using InfrastructureSqlite.PersistenceModels;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Jobs;

namespace InfrastructureSqlite.Mappers
{
    public static class JobPostPersistenceMapper
    {
        public static JobPostRecord ToRecord(JobPost jobPost)
        {
            return new JobPostRecord
            {
                Id = jobPost.Id,
                EmployerId = jobPost.EmployerId,
                Title = jobPost.Title.Value,
                Description = jobPost.Description,
                Requirement = jobPost.Requirement,
                WorkMode = jobPost.WorkMode.ToString(),
                MinSalary = jobPost.SalaryRange.MinSalary,
                MaxSalary = jobPost.SalaryRange.MaxSalary,
                SupportsWheelchairAccess = jobPost.AccessibilityInfo.SupportsWheelchairAccess,
                SupportsRemoteWork = jobPost.AccessibilityInfo.SupportsRemoteWork,
                SupportsFlexibleTime = jobPost.AccessibilityInfo.SupportsFlexibleTime,
                ProvidesAssistiveDevices = jobPost.AccessibilityInfo.ProvidesAssistiveDevices,
                AccessibilityAdditionalInfo = jobPost.AccessibilityInfo.AdditionalSupportDescription,
                Status = jobPost.Status.ToString(),
                CreatedAt = jobPost.CreatedAt,
                PublishedAt = jobPost.PublishedAt,
                ClosedAt = jobPost.ClosedAt
            };
        }

        public static JobPost ToDomain(JobPostRecord record)
        {
            var workMode = Enum.Parse<WorkMode>(record.WorkMode);
            var salaryRange = SalaryRange.Create(record.MinSalary, record.MaxSalary);
            var accessibilityInfo = JobAccessibilityInfo.Create(
                record.SupportsWheelchairAccess,
                record.SupportsRemoteWork,
                record.SupportsFlexibleTime,
                record.ProvidesAssistiveDevices,
                record.AccessibilityAdditionalInfo
            );
            var status = Enum.Parse<JobStatus>(record.Status);

            return JobPost.Restore(
                record.Id,
                record.EmployerId,
                record.Title,
                record.Description,
                record.Requirement,
                workMode,
                salaryRange,
                accessibilityInfo,
                status,
                record.CreatedAt,
                record.PublishedAt,
                record.ClosedAt
            );
        }

        public static void UpdateRecord(JobPostRecord record, JobPost jobPost)
        {
            record.Title = jobPost.Title.Value;
            record.Description = jobPost.Description;
            record.Requirement = jobPost.Requirement;
            record.WorkMode = jobPost.WorkMode.ToString();
            record.MinSalary = jobPost.SalaryRange.MinSalary;
            record.MaxSalary = jobPost.SalaryRange.MaxSalary;
            record.SupportsWheelchairAccess = jobPost.AccessibilityInfo.SupportsWheelchairAccess;
            record.SupportsRemoteWork = jobPost.AccessibilityInfo.SupportsRemoteWork;
            record.SupportsFlexibleTime = jobPost.AccessibilityInfo.SupportsFlexibleTime;
            record.ProvidesAssistiveDevices = jobPost.AccessibilityInfo.ProvidesAssistiveDevices;
            record.AccessibilityAdditionalInfo = jobPost.AccessibilityInfo.AdditionalSupportDescription;
            record.Status = jobPost.Status.ToString();
            record.PublishedAt = jobPost.PublishedAt;
            record.ClosedAt = jobPost.ClosedAt;
        }
    }
}
