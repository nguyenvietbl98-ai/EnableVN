using Domain.Jobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Jobs
{
    /// <summary>
    /// Query tìm kiếm job.
    /// 
    /// Các filter dạng nullable:
    /// - null: không lọc
    /// - true: chỉ lấy job có hỗ trợ tiêu chí đó
    /// </summary>
    public sealed class SearchJobQuery
    {
        public string? Keyword { get; init; }

        public WorkMode? WorkMode { get; init; }

        public bool? SupportsWheelchairAccess { get; init; }

        public bool? SupportsRemoteWork { get; init; }

        public bool? SupportsFlexibleTime { get; init; }

        public bool? ProvidesAssistiveDevices { get; init; }
    }
}
