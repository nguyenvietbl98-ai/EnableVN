using Domain.Users;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;

namespace Presentation.SeedData
{
    /// <summary>
    /// Seeder tạo tài khoản Admin mặc định cho môi trường Development/InMemory.
    /// 
    /// Vì InMemory lưu dữ liệu trong RAM, mỗi lần restart app dữ liệu sẽ mất.
    /// Seeder này giúp tự tạo lại Admin để bạn có thể đăng nhập quản trị.
    /// 
    /// Lưu ý:
    /// Không dùng hard-code password kiểu này cho production.
    /// </summary>
    public static class InMemoryAdminSeeder
    {
        public const string AdminEmail = "admin@enablevn.local";
        public const string AdminPassword = "Admin@123";

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            var existed = await userRepository.ExistsByEmailAsync(AdminEmail);

            if (existed)
                return;

            var admin = User.Register(
                AdminEmail,
                passwordHasher.Hash(AdminPassword),
                UserRole.Admin
            );

            await userRepository.AddAsync(admin);
        }
    }
}
