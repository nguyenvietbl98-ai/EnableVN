using Domain.Candidates;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureInMemory.Repositories
{
    /// <summary>
    /// Repository lưu CandidateProfile bằng RAM.
    /// 
    /// Có hỗ trợ lấy danh sách profile public cho phase 2.
    /// </summary>
    public sealed class InMemoryCandidateProfileRepository : ICandidateProfileRepository
    {
        private readonly List<CandidateProfile> _profiles = new();

        public Task<CandidateProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            var profile = _profiles.FirstOrDefault(x => x.Id == id);

            return Task.FromResult(profile);
        }

        public Task<CandidateProfile?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        )
        {
            var profile = _profiles.FirstOrDefault(x => x.UserId == userId);

            return Task.FromResult(profile);
        }

        public Task<bool> ExistsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        )
        {
            var exists = _profiles.Any(x => x.UserId == userId);

            return Task.FromResult(exists);
        }

        public Task<IReadOnlyList<CandidateProfile>> GetPublicProfilesAsync(
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<CandidateProfile> result = _profiles
                .Where(x => x.IsPublicProfile)
                .ToList();

            return Task.FromResult(result);
        }

        public Task AddAsync(
            CandidateProfile profile,
            CancellationToken cancellationToken = default
        )
        {
            _profiles.Add(profile);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(
            CandidateProfile profile,
            CancellationToken cancellationToken = default
        )
        {
            var index = _profiles.FindIndex(x => x.Id == profile.Id);

            if (index >= 0)
            {
                _profiles[index] = profile;
            }

            return Task.CompletedTask;
        }
    }
}
