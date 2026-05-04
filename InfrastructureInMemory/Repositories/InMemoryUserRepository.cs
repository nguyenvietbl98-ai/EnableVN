using Domain.Users;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureInMemory.Repositories
{
    /// <summary>
    /// Repository lưu User bằng RAM.
    /// 
    /// Dùng để test nhanh UseCase mà chưa cần database thật.
    /// Dữ liệu sẽ mất khi app restart.
    /// </summary>
    public sealed class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public Task<User?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            var user = _users.FirstOrDefault(x => x.Id == id);

            return Task.FromResult(user);
        }

        public Task<User?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default
        )
        {
            var normalizedEmail = email.Trim().ToLower();

            var user = _users.FirstOrDefault(x =>
                x.Email.Value == normalizedEmail
            );

            return Task.FromResult(user);
        }

        public Task<bool> ExistsByEmailAsync(
            string email,
            CancellationToken cancellationToken = default
        )
        {
            var normalizedEmail = email.Trim().ToLower();

            var exists = _users.Any(x =>
                x.Email.Value == normalizedEmail
            );

            return Task.FromResult(exists);
        }

        public Task AddAsync(
            User user,
            CancellationToken cancellationToken = default
        )
        {
            _users.Add(user);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(
            User user,
            CancellationToken cancellationToken = default
        )
        {
            var index = _users.FindIndex(x => x.Id == user.Id);

            if (index >= 0)
            {
                _users[index] = user;
            }

            return Task.CompletedTask;
        }
    }
}
