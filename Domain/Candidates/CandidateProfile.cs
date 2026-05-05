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
        public string? AvatarUrl { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public string? Gender { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? ContactEmail { get; private set; }
        public string? Address { get; private set; }
        public string? DesiredPosition { get; private set; }
        public decimal? DesiredSalary { get; private set; }
        public string? ExperienceSummary { get; private set; }
        public string? Skills { get; private set; }
        public string? Education { get; private set; }
        public string? Certifications { get; private set; }
        public string? PortfolioUrl { get; private set; }
        public string? Bio { get; private set; }
        public string? CvUrl { get; private set; }
        public string? JobSeekingStatus { get; private set; }
        public string? DesiredWorkMode { get; private set; }
        public string? AccessibilityNeeds { get; private set; }
        public DisabilityInfo DisabilityInfo { get; private set; }
        public bool IsPublicProfile { get; private set; }

        private CandidateProfile(
            Guid id,
            Guid userId,
            FullName fullName,
            string? avatarUrl,
            DateTime? dateOfBirth,
            string? gender,
            string? phoneNumber,
            string? contactEmail,
            string? address,
            string? desiredPosition,
            decimal? desiredSalary,
            string? experienceSummary,
            string? skills,
            string? education,
            string? certifications,
            string? portfolioUrl,
            string? bio,
            string? cvUrl,
            string? jobSeekingStatus,
            string? desiredWorkMode,
            string? accessibilityNeeds,
            DisabilityInfo disabilityInfo,
            bool isPublicProfile
        ) : base(id)
        {
            UserId = userId;
            FullName = fullName;
            AvatarUrl = avatarUrl;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            PhoneNumber = phoneNumber;
            ContactEmail = contactEmail;
            Address = address;
            DesiredPosition = desiredPosition;
            DesiredSalary = desiredSalary;
            ExperienceSummary = experienceSummary;
            Skills = skills;
            Education = education;
            Certifications = certifications;
            PortfolioUrl = portfolioUrl;
            Bio = bio;
            CvUrl = cvUrl;
            JobSeekingStatus = jobSeekingStatus;
            DesiredWorkMode = desiredWorkMode;
            AccessibilityNeeds = accessibilityNeeds;
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
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                bio,
                cvUrl,
                null,
                null,
                null,
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
            string? avatarUrl,
            DateTime? dateOfBirth,
            string? gender,
            string? phoneNumber,
            string? contactEmail,
            string? address,
            string? desiredPosition,
            decimal? desiredSalary,
            string? experienceSummary,
            string? skills,
            string? education,
            string? certifications,
            string? portfolioUrl,
            string? bio,
            string? cvUrl,
            string? jobSeekingStatus,
            string? desiredWorkMode,
            string? accessibilityNeeds
        )
        {
            FullName = FullName.Create(fullName);
            AvatarUrl = avatarUrl;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            PhoneNumber = phoneNumber;
            ContactEmail = contactEmail;
            Address = address;
            DesiredPosition = desiredPosition;
            DesiredSalary = desiredSalary;
            ExperienceSummary = experienceSummary;
            Skills = skills;
            Education = education;
            Certifications = certifications;
            PortfolioUrl = portfolioUrl;
            Bio = bio;
            CvUrl = cvUrl;
            JobSeekingStatus = jobSeekingStatus;
            DesiredWorkMode = desiredWorkMode;
            AccessibilityNeeds = accessibilityNeeds;
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
            string? avatarUrl,
            DateTime? dateOfBirth,
            string? gender,
            string? phoneNumber,
            string? contactEmail,
            string? address,
            string? desiredPosition,
            decimal? desiredSalary,
            string? experienceSummary,
            string? skills,
            string? education,
            string? certifications,
            string? portfolioUrl,
            string? bio,
            string? cvUrl,
            string? jobSeekingStatus,
            string? desiredWorkMode,
            string? accessibilityNeeds,
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
                avatarUrl,
                dateOfBirth,
                gender,
                phoneNumber,
                contactEmail,
                address,
                desiredPosition,
                desiredSalary,
                experienceSummary,
                skills,
                education,
                certifications,
                portfolioUrl,
                bio,
                cvUrl,
                jobSeekingStatus,
                desiredWorkMode,
                accessibilityNeeds,
                disabilityInfo,
                isPublicProfile
            );
        }
    }
}
