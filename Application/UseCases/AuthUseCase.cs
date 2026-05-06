using Application.Email;
using Application.Common;
using Domain.Users;
using Microsoft.Extensions.Logging;
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
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthUseCase> _logger;

        public AuthUseCase(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IDomainEventDispatcher domainEventDispatcher,
            IEmailService emailService,
            ILogger<AuthUseCase> logger
        )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _domainEventDispatcher = domainEventDispatcher;
            _emailService = emailService;
            _logger = logger;
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

            // FLOW 1 — Welcome email (best-effort, không làm fail đăng ký).
            _ = SendWelcomeEmailBestEffortAsync(
                recipientEmail: user.Email.Value,
                cancellationToken: cancellationToken);

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

        private async Task SendWelcomeEmailBestEffortAsync(
            string? recipientEmail,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                _logger.LogWarning("Welcome email skipped: recipient is empty.");
                return;
            }

            const string subject = "Chào mừng bạn đến với EnableVN";
            var html = EmailTemplates.RenderWelcomeEmailHtml(recipientEmail);

            try
            {
                await _emailService.SendAsync(recipientEmail, subject, html, cancellationToken);
            }
            catch (Exception ex)
            {
                // Best-effort: không làm fail đăng ký.
                _logger.LogWarning(
                    ex,
                    "Failed to send welcome email. Recipient={Recipient} Subject={Subject}",
                    recipientEmail,
                    subject);
            }
        }
    }
}
