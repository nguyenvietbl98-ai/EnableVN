using Domain.Applications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Applications
{
    /// <summary>
    /// Command dùng khi Employer đổi trạng thái hồ sơ ứng tuyển.
    /// </summary>
    public sealed class ChangeApplicationStatusCommand
    {
        public Guid ApplicationId { get; init; }

        public ApplicationStatus NewStatus { get; init; }

        public string? Note { get; init; }
    }
}
