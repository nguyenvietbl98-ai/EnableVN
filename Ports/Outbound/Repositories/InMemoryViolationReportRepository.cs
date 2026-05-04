using Domain.Reports;
using Ports.Outbound.Repositories;

namespace InfrastructureInMemory.Repositories
{
    public class InMemoryViolationReportRepository : IViolationReportRepository
    {
        private static readonly List<ViolationReport> _reports = new();

        public Task AddAsync(ViolationReport report, CancellationToken cancellationToken = default)
        {
            _reports.Add(report);
            return Task.CompletedTask;
        }

        public Task<ViolationReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var report = _reports.FirstOrDefault(r => r.Id == id);
            return Task.FromResult(report);
        }

        public Task UpdateAsync(ViolationReport report, CancellationToken cancellationToken = default)
        {
            var index = _reports.FindIndex(r => r.Id == report.Id);
            if (index != -1)
            {
                _reports[index] = report;
            }
            return Task.CompletedTask;
        }

        // ĐÃ SỬA: Đổi tên hàm thành GetByStatusAsync, thêm tham số ReportStatus 
        // và trả về IReadOnlyList (chuẩn chung của project Ports)
        public Task<IReadOnlyList<ViolationReport>> GetByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<ViolationReport> results = _reports.Where(r => r.Status == status).ToList();
            return Task.FromResult(results);
        }
    }
}