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
            return new CandidateProfileResult
            {
                Id = profile.Id,
                UserId = profile.UserId,
                FullName = profile.FullName.Value,
                Bio = profile.Bio,
                CvUrl = profile.CvUrl,

                DisabilityTypeId = canViewDisabilityInfo
                    ? profile.DisabilityInfo.DisabilityTypeId
                    : null,

                DisabilityDescription = canViewDisabilityInfo
                    ? profile.DisabilityInfo.Description
                    : null,

                IsDisabilityInfoVisibleToEmployer = profile.DisabilityInfo.IsVisibleToEmployer,
                IsPublicProfile = profile.IsPublicProfile
            };
        }
    }
}
