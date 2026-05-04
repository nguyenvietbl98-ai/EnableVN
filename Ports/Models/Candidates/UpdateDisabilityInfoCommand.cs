using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Candidates
{
    /// <summary>
    /// Command cập nhật thông tin khuyết tật.
    /// 
    /// IsVisibleToEmployer quyết định Employer có được xem thông tin này hay không.
    /// Đây là phần quan trọng về quyền riêng tư.
    /// </summary>
    public sealed class UpdateDisabilityInfoCommand
    {
        public Guid? DisabilityTypeId { get; init; }

        public string? Description { get; init; }

        public bool IsVisibleToEmployer { get; init; }
    }
}
