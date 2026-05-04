using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace InfrastructureInMemory.Repositories
{
    /// <summary>
    /// Password hasher đơn giản cho môi trường InMemory/demo.
    /// 
    /// Lưu ý:
    /// Đây KHÔNG phải lựa chọn tốt cho production.
    /// Production nên dùng BCrypt, Argon2, PBKDF2 hoặc ASP.NET Identity PasswordHasher.
    /// </summary>
    public sealed class SimplePasswordHasher : IPasswordHasher
    {
        public string Hash(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new InvalidOperationException("Mật khẩu không được để trống.");

            using var sha256 = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(plainPassword);
            var hashBytes = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hashBytes);
        }

        public bool Verify(string plainPassword, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                return false;

            if (string.IsNullOrWhiteSpace(passwordHash))
                return false;

            var hashOfInput = Hash(plainPassword);

            return hashOfInput == passwordHash;
        }
    }
}
