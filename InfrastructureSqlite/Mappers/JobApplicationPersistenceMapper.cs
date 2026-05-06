using InfrastructureSqlite.PersistenceModels;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Applications;

namespace InfrastructureSqlite.Mappers
{
    public static class JobApplicationPersistenceMapper
    {
        public static JobApplicationRecord ToRecord(JobApplication application)
        {
            return new JobApplicationRecord
            {
                Id = application.Id,
                JobId = application.JobId,
                CandidateId = application.CandidateId,
                CoverLetter = application.CoverLetter,
                CvUrl = application.CvUrl,
                Status = application.Status.ToString(),
                SubmittedAt = application.SubmittedAt,
                MatchScore = application.MatchScore,
                MatchLevel = application.MatchLevel,
                MatchReason = application.MatchReason
            };
        }

        public static JobApplication ToDomain(JobApplicationRecord record, IReadOnlyList<ApplicationStatusHistory> statusHistories)
        {
            var status = Enum.Parse<ApplicationStatus>(record.Status);

            return JobApplication.Restore(
                record.Id,
                record.JobId,
                record.CandidateId,
                record.CoverLetter,
                record.CvUrl,
                status,
                record.SubmittedAt,
                statusHistories,
                record.MatchScore,
                record.MatchLevel,
                record.MatchReason
            );
        }

        public static void UpdateRecord(JobApplicationRecord record, JobApplication application)
        {
            record.Status = application.Status.ToString();
            record.MatchScore = application.MatchScore;
            record.MatchLevel = application.MatchLevel;
            record.MatchReason = application.MatchReason;
        }
    }
}
