using Domain.Candidates;
using Ports.Models.Candidates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Mappers
{
    /// <summary>
    /// Mapper cho CandidateProfile.
    /// 
    /// Có tham số canViewDisabilityInfo để bảo vệ quyền riêng tư.
    /// Nếu Employer không được phép xem thông tin khuyết tật,
    /// Application không map phần nhạy cảm ra response.
    /// </summary>
    public static class CandidateProfileMapper
    {
        public static CandidateProfileResult ToResult(
            CandidateProfile profile,
            bool canViewDisabilityInfo
        )
        {
            var disabilityInfo = profile.DisabilityInfo;

            return new CandidateProfileResult
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

                DisabilityTypeId = canViewDisabilityInfo
                    ? disabilityInfo?.DisabilityTypeId
                    : null,

                DisabilityDescription = canViewDisabilityInfo
                    ? disabilityInfo?.Description
                    : null,

                IsDisabilityInfoVisibleToEmployer = disabilityInfo?.IsVisibleToEmployer ?? false,
                IsPublicProfile = profile.IsPublicProfile
            };
        }
    }
}
