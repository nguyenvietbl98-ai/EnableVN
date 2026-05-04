using Domain.Catalogs;
using Microsoft.Extensions.DependencyInjection;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureSqlite.SeedData
{
    /// <summary>
    /// Seeder cho danh mục mặc định: DisabilityType, AssistiveDevice, JobCategory.
    /// 
    /// Chỉ tạo một lần, không tạo trùng lần sau.
    /// </summary>
    public static class SqliteCatalogSeeder
    {
        public static async Task SeedAsync(
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
        {
            using var scope = serviceProvider.CreateScope();

            var disabilityTypeRepo = scope.ServiceProvider.GetRequiredService<IDisabilityTypeRepository>();
            var assistiveDeviceRepo = scope.ServiceProvider.GetRequiredService<IAssistiveDeviceRepository>();
            var jobCategoryRepo = scope.ServiceProvider.GetRequiredService<IJobCategoryRepository>();

            // Seed DisabilityTypes
            await SeedDisabilityTypesAsync(disabilityTypeRepo, cancellationToken);

            // Seed AssistiveDevices
            await SeedAssistiveDevicesAsync(assistiveDeviceRepo, cancellationToken);

            // Seed JobCategories
            await SeedJobCategoriesAsync(jobCategoryRepo, cancellationToken);
        }

        private static async Task SeedDisabilityTypesAsync(
            IDisabilityTypeRepository repo,
            CancellationToken cancellationToken)
        {
            var existing = await repo.GetAllAsync(cancellationToken);

            if (existing.Count > 0)
            {
                return; // Đã có dữ liệu, không tạo lại
            }

            var types = new[]
            {
                ("Khiếm thị", "Người có thị lực bị giảm sút hoặc mù hoàn toàn"),
                ("Khiếm thính", "Người có thính lực bị giảm sút hoặc điếc"),
                ("Khiếm động", "Người có khả năng vận động bị hạn chế"),
                ("Khiếm khôn", "Người có rối loạn trí tuệ hoặc phát triển"),
                ("Tự kỷ", "Người có chứng tự kỷ phổ biến"),
                ("Rối loạn tâm lý", "Người có rối loạn sức khỏe tâm thần"),
                ("Bệnh mãn tính", "Người sống chung với bệnh mãn tính")
            };

            foreach (var (name, description) in types)
            {
                var disabilityType = DisabilityType.Create(name, description);
                await repo.AddAsync(disabilityType, cancellationToken);
            }

            Console.WriteLine($"✓ Seeded {types.Length} DisabilityTypes");
        }

        private static async Task SeedAssistiveDevicesAsync(
            IAssistiveDeviceRepository repo,
            CancellationToken cancellationToken)
        {
            var existing = await repo.GetAllAsync(cancellationToken);

            if (existing.Count > 0)
            {
                return;
            }

            var devices = new[]
            {
                ("Màn hình nói", "Phần mềm đọc màn hình cho người khiếm thị"),
                ("Bàn phím đặc biệt", "Bàn phím được thiết kế cho những người khó sử dụng bàn phím thông thường"),
                ("Chuột đặc biệt", "Chuột được thiết kế cho khả năng vận động hạn chế"),
                ("Thiết bị trợ thính", "Máy trợ thính và thiết bị liên quan"),
                ("Bảng chữ cái mở rộng", "Bộ lọc hoặc phần mềm nhận dạng giọng nói"),
                ("Tựa gối", "Thiết bị hỗ trợ tư thế khi làm việc"),
                ("Xe lăn", "Xe lăn và thiết bị di chuyển")
            };

            foreach (var (name, description) in devices)
            {
                var device = AssistiveDevice.Create(name, description);
                await repo.AddAsync(device, cancellationToken);
            }

            Console.WriteLine($"✓ Seeded {devices.Length} AssistiveDevices");
        }

        private static async Task SeedJobCategoriesAsync(
            IJobCategoryRepository repo,
            CancellationToken cancellationToken)
        {
            var existing = await repo.GetAllAsync(cancellationToken);

            if (existing.Count > 0)
            {
                return;
            }

            var categories = new[]
            {
                ("Công nghệ thông tin", "Phần mềm, phần cứng, CNTT"),
                ("Bán hàng", "Bán hàng, kinh doanh, tiếp thị"),
                ("Kế toán", "Kế toán, tài chính, kiểm toán"),
                ("Nhân sự", "Tuyển dụng, đào tạo, nhân sự"),
                ("Chăm sóc khách hàng", "Hỗ trợ khách hàng, dịch vụ"),
                ("Thiết kế", "Thiết kế đồ họa, UI/UX"),
                ("Quản lý dự án", "Quản lý, điều phối, lập kế hoạch"),
                ("Giáo dục", "Dạy học, tư vấn, huấn luyện")
            };

            foreach (var (name, description) in categories)
            {
                var category = JobCategory.Create(name, description);
                await repo.AddAsync(category, cancellationToken);
            }

            Console.WriteLine($"✓ Seeded {categories.Length} JobCategories");
        }
    }
}
