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
                Bio = profile.Bio,
                CvUrl = profile.CvUrl,
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
                record.Bio,
                record.CvUrl,
                disabilityInfo,
                record.IsPublicProfile
            );
        }

        public static void UpdateRecord(CandidateProfileRecord record, CandidateProfile profile)
        {
            record.FullName = profile.FullName.Value;
            record.Bio = profile.Bio;
            record.CvUrl = profile.CvUrl;
            record.DisabilityTypeId = profile.DisabilityInfo.DisabilityTypeId;
            record.DisabilityDescription = profile.DisabilityInfo.Description;
            record.IsDisabilityVisibleToEmployer = profile.DisabilityInfo.IsVisibleToEmployer;
            record.IsPublicProfile = profile.IsPublicProfile;
        }
    }
}
