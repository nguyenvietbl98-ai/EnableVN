using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Candidates
{
    // Query này chứa điều kiện tìm kiếm ứng viên public.
    public sealed class SearchPublicCandidatesQuery
    {
        public string? Keyword { get; set; } // Tìm theo tên, bio.

        public Guid? DisabilityTypeId { get; set; } // Lọc theo loại khuyết tật nếu ứng viên cho phép hiển thị.

        public bool? HasCv { get; set; } // Lọc ứng viên có CV.
    }
}
