using Domain.Users;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureInMemory.Repositories
{
    /// <summary>
    /// Token service đơn giản cho InMemory/demo.
    /// 
    /// Lưu ý:
    /// Đây không phải JWT thật.
    /// Production sẽ implement ITokenService bằng JWT hoặc cơ chế token an toàn khác.
    /// </summary>
    public sealed class SimpleTokenService : ITokenService
    {
        public string GenerateToken(Guid userId, string email, UserRole role)
        {
            var raw = $"{userId}|{email}|{role}|{DateTime.UtcNow:O}";

            var bytes = Encoding.UTF8.GetBytes(raw);

            return Convert.ToBase64String(bytes);
        }
    }
}
