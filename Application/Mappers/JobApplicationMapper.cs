using Domain.Applications;
using Ports.Models.Applications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Mappers
{
    /// <summary>
    /// Mapper cho JobApplication.
    /// </summary>
    public static class JobApplicationMapper
    {
        private const string SystemSubmitNote = "Ứng viên đã nộp hồ sơ.";
        private const string SystemWithdrawNote = "Ứng viên đã rút hồ sơ.";

        public static JobApplicationResult ToResult(JobApplication application)
        {
            var ordered = application.StatusHistories
                .OrderBy(h => h.ChangedAt)
                .ToList();

            var last = ordered.Count > 0 ? ordered[^1] : null;

            var employerFeedback = ordered
                .Where(h =>
                    !string.IsNullOrWhiteSpace(h.Note) &&
                    h.Note != SystemSubmitNote &&
                    h.Note != SystemWithdrawNote)
                .LastOrDefault();

            return new JobApplicationResult
            {
                Id = application.Id,
                JobId = application.JobId,
                CandidateId = application.CandidateId,
                CoverLetter = application.CoverLetter,
                CvUrl = application.CvUrl,
                Status = application.Status,
                SubmittedAt = application.SubmittedAt,
                LatestHistoryNote = last?.Note,
                LatestHistoryStatus = last?.Status,
                LatestHistoryAt = last?.ChangedAt,
                EmployerFeedbackNote = employerFeedback?.Note,
                EmployerFeedbackAt = employerFeedback?.ChangedAt
            };
        }
    }
}
