using InfrastructureSqlite.PersistenceModels;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Candidates;

namespace InfrastructureSqlite.Mappers
{
    public static class CandidateProfilePersistenceMapper
    {
        public static CandidateProfileRecord ToRecord(CandidateProfile profile)
        {
            return new CandidateProfileRecord
            {
                Id = profile.Id,
                UserId = profile.UserId,
                FullName = profile.FullName.Value,
                AvatarUrl = profile.AvatarUrl,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                PhoneNumber = profile.PhoneNumber,
                ContactEmail = profile.ContactEmail,
                Address = profile.Address,
                DesiredPosition = profile.DesiredPosition,
                DesiredSalary = profile.DesiredSalary,
                ExperienceSummary = profile.ExperienceSummary,
                Skills = profile.Skills,
                Education = profile.Education,
                Certifications = profile.Certifications,
                PortfolioUrl = profile.PortfolioUrl,
                Bio = profile.Bio,
                CvUrl = profile.CvUrl,
                JobSeekingStatus = profile.JobSeekingStatus,
                DesiredWorkMode = profile.DesiredWorkMode,
                AccessibilityNeeds = profile.AccessibilityNeeds,
                DisabilityTypeId = profile.DisabilityInfo.DisabilityTypeId,
                DisabilityDescription = profile.DisabilityInfo.Description,
                IsDisabilityVisibleToEmployer = profile.DisabilityInfo.IsVisibleToEmployer,
                IsPublicProfile = profile.IsPublicProfile
            };
        }

        public static CandidateProfile ToDomain(CandidateProfileRecord record)
        {
            var disabilityInfo = DisabilityInfo.Create(
                record.DisabilityTypeId,
                record.DisabilityDescription,
                record.IsDisabilityVisibleToEmployer
            );

            return CandidateProfile.Restore(
                record.Id,
                record.UserId,
                record.FullName,
                record.AvatarUrl,
                record.DateOfBirth,
                record.Gender,
                record.PhoneNumber,
                record.ContactEmail,
                record.Address,
                record.DesiredPosition,
                record.DesiredSalary,
                record.ExperienceSummary,
                record.Skills,
                record.Education,
                record.Certifications,
                record.PortfolioUrl,
                record.Bio,
                record.CvUrl,
                record.JobSeekingStatus,
                record.DesiredWorkMode,
                record.AccessibilityNeeds,
                disabilityInfo,
                record.IsPublicProfile
            );
        }

        public static void UpdateRecord(CandidateProfileRecord record, CandidateProfile profile)
        {
            record.FullName = profile.FullName.Value;
            record.AvatarUrl = profile.AvatarUrl;
            record.DateOfBirth = profile.DateOfBirth;
            record.Gender = profile.Gender;
            record.PhoneNumber = profile.PhoneNumber;
            record.ContactEmail = profile.ContactEmail;
            record.Address = profile.Address;
            record.DesiredPosition = profile.DesiredPosition;
            record.DesiredSalary = profile.DesiredSalary;
            record.ExperienceSummary = profile.ExperienceSummary;
            record.Skills = profile.Skills;
            record.Education = profile.Education;
            record.Certifications = profile.Certifications;
            record.PortfolioUrl = profile.PortfolioUrl;
            record.Bio = profile.Bio;
            record.CvUrl = profile.CvUrl;
            record.JobSeekingStatus = profile.JobSeekingStatus;
            record.DesiredWorkMode = profile.DesiredWorkMode;
            record.AccessibilityNeeds = profile.AccessibilityNeeds;
            record.DisabilityTypeId = profile.DisabilityInfo.DisabilityTypeId;
            record.DisabilityDescription = profile.DisabilityInfo.Description;
            record.IsDisabilityVisibleToEmployer = profile.DisabilityInfo.IsVisibleToEmployer;
            record.IsPublicProfile = profile.IsPublicProfile;
        }
    }
}
