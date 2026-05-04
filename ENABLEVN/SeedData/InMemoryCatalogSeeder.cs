using Domain.Catalogs;
using Ports.Outbound.Repositories;

namespace Presentation.SeedData
{
    /// <summary>
    /// Seeder tạo dữ liệu danh mục mặc định cho môi trường Development/InMemory.
    /// 
    /// Vì InMemory lưu dữ liệu trong RAM, mỗi lần restart app dữ liệu sẽ mất.
    /// Seeder này giúp hệ thống có sẵn danh mục cơ bản để test UI và flow MVP.
    /// 
    /// Chỉ dùng cho Development/Demo.
    /// Không dùng làm cơ chế seed production.
    /// </summary>
    public static class InMemoryCatalogSeeder
    {
        /// <summary>
        /// Seed toàn bộ catalog mặc định.
        /// </summary>
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var disabilityTypeRepository = scope.ServiceProvider
                .GetRequiredService<IDisabilityTypeRepository>();

            var assistiveDeviceRepository = scope.ServiceProvider
                .GetRequiredService<IAssistiveDeviceRepository>();

            var jobCategoryRepository = scope.ServiceProvider
                .GetRequiredService<IJobCategoryRepository>();

            await SeedDisabilityTypesAsync(disabilityTypeRepository);
            await SeedAssistiveDevicesAsync(assistiveDeviceRepository);
            await SeedJobCategoriesAsync(jobCategoryRepository);
        }

        /// <summary>
        /// Seed danh mục loại khuyết tật mặc định.
        /// 
        /// Nếu repository đã có dữ liệu thì không seed lại,
        /// tránh tạo trùng trong cùng một lần chạy app.
        /// </summary>
        private static async Task SeedDisabilityTypesAsync(
            IDisabilityTypeRepository repository
        )
        {
            var existingItems = await repository.GetAllAsync();

            if (existingItems.Count > 0)
                return;

            var items = new[]
            {
            DisabilityType.Create(
                "Khiếm thị",
                "Ứng viên có khó khăn về thị giác, cần hỗ trợ như screen reader, tương phản cao hoặc tài liệu dễ đọc."
            ),
            DisabilityType.Create(
                "Khiếm thính",
                "Ứng viên có khó khăn về thính giác, có thể cần phụ đề, giao tiếp bằng văn bản hoặc môi trường họp phù hợp."
            ),
            DisabilityType.Create(
                "Khuyết tật vận động",
                "Ứng viên có khó khăn về vận động, có thể cần hỗ trợ xe lăn, lối đi tiếp cận hoặc làm việc từ xa."
            ),
            DisabilityType.Create(
                "Khuyết tật trí tuệ",
                "Ứng viên cần hướng dẫn rõ ràng, quy trình đơn giản và môi trường làm việc hỗ trợ."
            ),
            DisabilityType.Create(
                "Rối loạn phổ tự kỷ",
                "Ứng viên có thể cần môi trường làm việc yên tĩnh, lịch trình rõ ràng hoặc giao tiếp trực tiếp, cụ thể."
            ),
            DisabilityType.Create(
                "Khác",
                "Các nhu cầu hỗ trợ khác không nằm trong các nhóm trên."
            )
        };

            foreach (var item in items)
            {
                await repository.AddAsync(item);
            }
        }

        /// <summary>
        /// Seed danh mục thiết bị hỗ trợ mặc định.
        /// </summary>
        private static async Task SeedAssistiveDevicesAsync(
            IAssistiveDeviceRepository repository
        )
        {
            var existingItems = await repository.GetAllAsync();

            if (existingItems.Count > 0)
                return;

            var items = new[]
            {
            AssistiveDevice.Create(
                "Screen reader",
                "Phần mềm đọc màn hình hỗ trợ người khiếm thị sử dụng máy tính và website."
            ),
            AssistiveDevice.Create(
                "Xe lăn",
                "Thiết bị hỗ trợ di chuyển cho người khuyết tật vận động."
            ),
            AssistiveDevice.Create(
                "Thiết bị trợ thính",
                "Thiết bị hỗ trợ nghe hoặc khuếch đại âm thanh."
            ),
            AssistiveDevice.Create(
                "Bàn phím đặc biệt",
                "Bàn phím hoặc thiết bị nhập liệu phù hợp với người có hạn chế vận động."
            ),
            AssistiveDevice.Create(
                "Phụ đề / caption",
                "Hỗ trợ nội dung âm thanh hoặc cuộc họp bằng phụ đề."
            ),
            AssistiveDevice.Create(
                "Khác",
                "Các thiết bị hoặc công cụ hỗ trợ khác."
            )
        };

            foreach (var item in items)
            {
                await repository.AddAsync(item);
            }
        }

        /// <summary>
        /// Seed danh mục ngành nghề mặc định.
        /// </summary>
        private static async Task SeedJobCategoriesAsync(
            IJobCategoryRepository repository
        )
        {
            var existingItems = await repository.GetAllAsync();

            if (existingItems.Count > 0)
                return;

            var items = new[]
            {
            JobCategory.Create(
                "Công nghệ thông tin",
                "Các vị trí liên quan đến lập trình, kiểm thử, dữ liệu, hệ thống và sản phẩm công nghệ."
            ),
            JobCategory.Create(
                "Kế toán",
                "Các vị trí kế toán, kiểm toán, tài chính nội bộ và xử lý chứng từ."
            ),
            JobCategory.Create(
                "Marketing",
                "Các vị trí tiếp thị, nội dung, truyền thông, quảng cáo và phát triển thương hiệu."
            ),
            JobCategory.Create(
                "Chăm sóc khách hàng",
                "Các vị trí hỗ trợ khách hàng, tư vấn, tổng đài và vận hành dịch vụ."
            ),
            JobCategory.Create(
                "Hành chính - Nhân sự",
                "Các vị trí hành chính văn phòng, tuyển dụng, nhân sự và vận hành nội bộ."
            ),
            JobCategory.Create(
                "Thiết kế",
                "Các vị trí thiết kế đồ họa, UI/UX, thiết kế sản phẩm và sáng tạo hình ảnh."
            ),
            JobCategory.Create(
                "Bán hàng",
                "Các vị trí kinh doanh, tư vấn bán hàng và phát triển khách hàng."
            ),
            JobCategory.Create(
                "Khác",
                "Các ngành nghề khác chưa được phân loại."
            )
        };

            foreach (var item in items)
            {
                await repository.AddAsync(item);
            }
        }
    }
}
