using Application.Common;
using Domain.Users;
using Ports.Inbound;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases
{
    /// <summary>
    /// UseCase quản lý User.
    /// Chủ yếu dành cho Admin.
    /// </summary>
    public sealed class UserUseCase : IUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;

        public UserUseCase(
            IUserRepository userRepository,
            ICurrentUserService currentUser
        )
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        public async Task LockUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var user = await GetUserOrThrowAsync(userId, cancellationToken);

            user.Lock();

            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        public async Task ActivateUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var user = await GetUserOrThrowAsync(userId, cancellationToken);

            user.Activate();

            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        public async Task DeleteUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var user = await GetUserOrThrowAsync(userId, cancellationToken);

            user.Delete();

            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        public async Task<UserRole?> GetUserRoleAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAuthenticatedUser(_currentUser);

            var user = await _userRepository.GetByIdAsync(
                userId,
                cancellationToken
            );

            return user?.Role;
        }

        private async Task<User> GetUserOrThrowAsync(
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            var user = await _userRepository.GetByIdAsync(
                userId,
                cancellationToken
            );

            if (user is null)
                throw new UseCaseException("Không tìm thấy tài khoản.");

            return user;
        }
    }
}
