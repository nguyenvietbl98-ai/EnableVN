using System;
using System.Collections.Generic;
using System.Text;
using Domain.Common;
using Domain.Users.Events;
namespace Domain.Users
{


    public sealed class User : AggregateRoot<Guid>
    {
        public Email Email { get; private set; }
        public string PasswordHash { get; private set; }
        public UserRole Role { get; private set; }
        public UserStatus Status { get; private set; }

        private User(
            Guid id,
            Email email,
            string passwordHash,
            UserRole role
        ) : base(id)
        {
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            Status = UserStatus.Active;
        }

        public static User Register(
            string email,
            string passwordHash,
            UserRole role
        )
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Mật khẩu đã mã hóa không được để trống.");

            var user = new User(
                Guid.NewGuid(),
                Email.Create(email),
                passwordHash,
                role
            );

            user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.Role));

            return user;
        }

        public void Lock()
        {
            if (Status == UserStatus.Deleted)
                throw new DomainException("Không thể khóa tài khoản đã bị xóa.");

            Status = UserStatus.Locked;
        }

        public void Activate()
        {
            if (Status == UserStatus.Deleted)
                throw new DomainException("Không thể kích hoạt tài khoản đã bị xóa.");

            Status = UserStatus.Active;
        }

        public void Delete()
        {
            Status = UserStatus.Deleted;
        }
        public static User Restore(
    Guid id,
    string email,
    string passwordHash,
    UserRole role,
    UserStatus status)
        {
            if (id == Guid.Empty)
                throw new DomainException("UserId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Mật khẩu đã mã hóa không được để trống.");

            var user = new User(
                id,
                Email.Create(email),
                passwordHash,
                role);

            user.Status = status;

            return user;
        }
    }
}
