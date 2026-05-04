using Domain.Employers;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureInMemory.Repositories
{
    /// <summary>
    /// Repository lưu EmployerProfile bằng RAM.
    /// </summary>
    public sealed class InMemoryEmployerProfileRepository : IEmployerProfileRepository
    {
        private readonly List<EmployerProfile> _profiles = new();

        public Task<EmployerProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            var profile = _profiles.FirstOrDefault(x => x.Id == id);

            return Task.FromResult(profile);
        }

        public Task<EmployerProfile?> GetByUserIdAsync(
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

        public Task AddAsync(
            EmployerProfile profile,
            CancellationToken cancellationToken = default
        )
        {
            _profiles.Add(profile);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(
            EmployerProfile profile,
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
