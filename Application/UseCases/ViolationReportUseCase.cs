using Application.Common;
using Domain.Reports;
using Ports.Inbound;
using Ports.Models.Reports;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases
{
    public sealed class ViolationReportUseCase : IViolationReportUseCase
    {
        private readonly IViolationReportRepository _reportRepository;
        private readonly ICurrentUserService _currentUser;

        public ViolationReportUseCase(
            IViolationReportRepository reportRepository,
            ICurrentUserService currentUser)
        {
            _reportRepository = reportRepository;
            _currentUser = currentUser;
        }

        public async Task<Guid> CreateAsync(
            CreateViolationReportCommand command,
            CancellationToken cancellationToken = default)
        {
            var reporterUserId = AuthorizationGuard.RequireAuthenticatedUser(_currentUser);
            // Chỉ cần đăng nhập là được gửi báo cáo.
            // Không phân biệt Admin, Employer hay Candidate.

            var report = ViolationReport.Create(
                reporterUserId,
                command.TargetType,
                command.TargetId,
                command.Reason
            );
            // Tạo Domain Entity report.
            // Mặc định status sẽ là Pending.

            await _reportRepository.AddAsync(
                report,
                cancellationToken
            );
            // Lưu report vào database.

            return report.Id;
        }

        public async Task<IReadOnlyList<ViolationReportResult>> GetPendingReportsAsync(
            CancellationToken cancellationToken = default)
        {
            AuthorizationGuard.RequireAdmin(_currentUser);
            // Chỉ Admin được xem danh sách report đang chờ xử lý.

            var reports = await _reportRepository.GetByStatusAsync(
                ReportStatus.Pending,
                cancellationToken
            );

            return reports
                .Select(x => new ViolationReportResult
                {
                    Id = x.Id,
                    ReporterUserId = x.ReporterUserId,
                    TargetType = x.TargetType,
                    TargetId = x.TargetId,
                    Reason = x.Reason,
                    Status = x.Status,
                    AdminNote = x.AdminNote,
                    CreatedAt = x.CreatedAt,
                    HandledAt = x.HandledAt
                })
                .ToList();
        }

        public async Task ResolveAsync(
            HandleViolationReportCommand command,
            CancellationToken cancellationToken = default)
        {
            AuthorizationGuard.RequireAdmin(_currentUser);
            // Chỉ Admin được xác nhận và xử lý báo cáo.

            var report = await _reportRepository.GetByIdAsync(
                command.ReportId,
                cancellationToken
            );

            if (report is null)
                throw new UseCaseException("Không tìm thấy báo cáo.");

            report.Resolve(command.AdminNote);
            // Gọi method trong Domain Entity.
            // Domain tự kiểm tra nếu report đã xử lý rồi thì không cho xử lý lại.

            await _reportRepository.UpdateAsync(
                report,
                cancellationToken
            );
            // Lưu trạng thái mới xuống database.
        }

        public async Task RejectAsync(
            HandleViolationReportCommand command,
            CancellationToken cancellationToken = default)
        {
            AuthorizationGuard.RequireAdmin(_currentUser);
            // Chỉ Admin được từ chối báo cáo.

            var report = await _reportRepository.GetByIdAsync(
                command.ReportId,
                cancellationToken
            );

            if (report is null)
                throw new UseCaseException("Không tìm thấy báo cáo.");

            report.Reject(command.AdminNote);
            // Đổi trạng thái report sang Rejected.

            await _reportRepository.UpdateAsync(
                report,
                cancellationToken
            );
            // Lưu thay đổi.
        }
    }
}
