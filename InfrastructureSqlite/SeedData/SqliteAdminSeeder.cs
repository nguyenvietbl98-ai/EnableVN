using Domain.Catalogs;
using Microsoft.Extensions.DependencyInjection;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureSqlite.SeedData
{
    /// <summary>
    /// Seeder cho Admin mặc định trong môi trường Development/SQLite.
    /// 
    /// Email: admin@enablevn.local
    /// Password: Admin@123
    /// 
    /// Chỉ tạo một lần, không tạo trùng lần sau.
    /// </summary>
    public static class SqliteAdminSeeder
    {
        public static async Task SeedAsync(
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
        {
            using var scope = serviceProvider.CreateScope();

            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            var adminEmail = "admin@enablevn.local";

            // Kiểm tra xem Admin đã tồn tại chưa
            var existingAdmin = await userRepository.GetByEmailAsync(adminEmail, cancellationToken);
            if (existingAdmin is not null)
            {
                return; // Đã tồn tại, không tạo lại
            }

            // Tạo admin mới
            var passwordHash = passwordHasher.Hash("Admin@123");

            var admin = Domain.Users.User.Register(
                adminEmail,
                passwordHash,
                Domain.Users.UserRole.Admin
            );

            await userRepository.AddAsync(admin, cancellationToken);

            Console.WriteLine("✓ Seeded default Admin: admin@enablevn.local / Admin@123");
        }
    }
}
