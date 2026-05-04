using Application.Common;
using Domain.Users;
using Ports.Inbound;
using Ports.Models.Auth;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases
{
    /// <summary>
    /// Application Service thực thi nghiệp vụ xác thực.
    /// 
    /// Implement Inbound Port: IAuthUseCase.
    /// 
    /// Nhiệm vụ:
    /// - Đăng ký tài khoản
    /// - Đăng nhập
    /// - Hash/verify password thông qua Outbound Port
    /// - Tạo token thông qua Outbound Port
    /// </summary>
    public sealed class AuthUseCase : IAuthUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public AuthUseCase(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IDomainEventDispatcher domainEventDispatcher
        )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public async Task<AuthResult> RegisterAsync(
            RegisterCommand command,
            CancellationToken cancellationToken = default
        )
        {
            if (command.Role == UserRole.Admin)
                throw new UseCaseException("Không thể đăng ký tài khoản Admin từ luồng public.");

            var emailExists = await _userRepository.ExistsByEmailAsync(
                command.Email,
                cancellationToken
            );

            if (emailExists)
                throw new UseCaseException("Email đã tồn tại trong hệ thống.");

            var passwordHash = _passwordHasher.Hash(command.Password);

            var user = User.Register(
                command.Email,
                passwordHash,
                command.Role
            );

            await _userRepository.AddAsync(user, cancellationToken);

            await DomainEventHelper.DispatchAndClearEventsAsync(
                user,
                _domainEventDispatcher,
                cancellationToken
            );

            var token = _tokenService.GenerateToken(
                user.Id,
                user.Email.Value,
                user.Role
            );

            return new AuthResult
            {
                UserId = user.Id,
                Email = user.Email.Value,
                Role = user.Role,
                Token = token
            };
        }

        public async Task<AuthResult> LoginAsync(
            LoginCommand command,
            CancellationToken cancellationToken = default
        )
        {
            var user = await _userRepository.GetByEmailAsync(
                command.Email,
                cancellationToken
            );

            if (user is null)
                throw new UseCaseException("Email hoặc mật khẩu không đúng.");

            if (user.Status != UserStatus.Active)
                throw new UseCaseException("Tài khoản không ở trạng thái hoạt động.");

            var isPasswordValid = _passwordHasher.Verify(
                command.Password,
                user.PasswordHash
            );

            if (!isPasswordValid)
                throw new UseCaseException("Email hoặc mật khẩu không đúng.");

            var token = _tokenService.GenerateToken(
                user.Id,
                user.Email.Value,
                user.Role
            );

            return new AuthResult
            {
                UserId = user.Id,
                Email = user.Email.Value,
                Role = user.Role,
                Token = token
            };
        }
    }
}
