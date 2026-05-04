using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Candidates.Events;

namespace Domain.Candidates
{
    public sealed class CandidateProfile : AggregateRoot<Guid>
    {
        public Guid UserId { get; private set; }
        public FullName FullName { get; private set; }
        public string? Bio { get; private set; }
        public string? CvUrl { get; private set; }
        public DisabilityInfo DisabilityInfo { get; private set; }
        public bool IsPublicProfile { get; private set; }

        private CandidateProfile(
            Guid id,
            Guid userId,
            FullName fullName,
            string? bio,
            string? cvUrl,
            DisabilityInfo disabilityInfo,
            bool isPublicProfile
        ) : base(id)
        {
            UserId = userId;
            FullName = fullName;
            Bio = bio;
            CvUrl = cvUrl;
            DisabilityInfo = disabilityInfo;
            IsPublicProfile = isPublicProfile;
        }

        public static CandidateProfile Create(
            Guid userId,
            string fullName,
            string? bio,
            string? cvUrl
        )
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId của ứng viên không hợp lệ.");

            var profile = new CandidateProfile(
                Guid.NewGuid(),
                userId,
                FullName.Create(fullName),
                bio,
                cvUrl,
                DisabilityInfo.Hidden(),
                false
            );

            profile.AddDomainEvent(
                new CandidateProfileCreatedEvent(profile.Id, userId)
            );

            return profile;
        }

        public void UpdateBasicInfo(
            string fullName,
            string? bio,
            string? cvUrl
        )
        {
            FullName = FullName.Create(fullName);
            Bio = bio;
            CvUrl = cvUrl;
        }

        public void UpdateDisabilityInfo(DisabilityInfo disabilityInfo)
        {
            DisabilityInfo = disabilityInfo;
        }

        public void HideDisabilityInfo()
        {
            DisabilityInfo.Hide();
        }

        public void ShowDisabilityInfoToEmployer()
        {
            DisabilityInfo.Show();
        }

        public void MakeProfilePublic()
        {
            IsPublicProfile = true;
        }

        public void MakeProfilePrivate()
        {
            IsPublicProfile = false;
        }

        public static CandidateProfile Restore(
            Guid id,
            Guid userId,
            string fullName,
            string? bio,
            string? cvUrl,
            DisabilityInfo disabilityInfo,
            bool isPublicProfile
        )
        {
            if (id == Guid.Empty)
                throw new DomainException("CandidateProfileId không hợp lệ.");

            if (userId == Guid.Empty)
                throw new DomainException("UserId không hợp lệ.");

            return new CandidateProfile(
                id,
                userId,
                FullName.Create(fullName),
                bio,
                cvUrl,
                disabilityInfo,
                isPublicProfile
            );
        }
    }
}
