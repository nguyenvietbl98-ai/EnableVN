using Domain.Jobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Jobs
{
    /// <summary>
    /// Command tạo job dạng Draft.
    /// EmployerId không nhận từ client, nên lấy từ current user rồi tìm EmployerProfile.
    /// </summary>
    public sealed class CreateJobCommand
    {
        public string Title { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public string Requirement { get; init; } = string.Empty;

        public WorkMode WorkMode { get; init; }

        public decimal? MinSalary { get; init; }

        public decimal? MaxSalary { get; init; }

        public bool SupportsWheelchairAccess { get; init; }

        public bool SupportsRemoteWork { get; init; }

        public bool SupportsFlexibleTime { get; init; }

        public bool ProvidesAssistiveDevices { get; init; }

        public string? AdditionalSupportDescription { get; init; }
    }
}
