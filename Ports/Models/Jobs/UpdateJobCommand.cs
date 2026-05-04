using Domain.Jobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Jobs
{
    /// <summary>
    /// Command cập nhật nội dung job.
    /// </summary>
    public sealed class UpdateJobCommand
    {
        public Guid JobId { get; init; }

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
