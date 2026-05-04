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
        public static JobApplicationResult ToResult(JobApplication application)
        {
            return new JobApplicationResult
            {
                Id = application.Id,
                JobId = application.JobId,
                CandidateId = application.CandidateId,
                CoverLetter = application.CoverLetter,
                CvUrl = application.CvUrl,
                Status = application.Status,
                SubmittedAt = application.SubmittedAt
            };
        }
    }
}
