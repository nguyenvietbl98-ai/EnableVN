using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Reviews
{
    public sealed class CreateCompanyReviewCommand
    {
        public Guid EmployerId { get; set; }
        // Id của EmployerProfile được đánh giá.

        public int Rating { get; set; }
        // Điểm đánh giá từ 1 đến 5.

        public string Comment { get; set; } = string.Empty;
        // Nội dung nhận xét của ứng viên.
    }
}
