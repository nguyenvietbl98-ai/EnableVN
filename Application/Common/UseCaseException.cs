using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common
{
    /// <summary>
    /// Exception riêng cho tầng Application.
    /// 
    /// Dùng cho các lỗi liên quan đến use case, phân quyền,
    /// dữ liệu không tồn tại, hoặc thao tác không hợp lệ ở cấp ứng dụng.
    /// 
    /// Khác với DomainException:
    /// - DomainException: lỗi rule nghiệp vụ thuần trong Domain.
    /// - UseCaseException: lỗi điều phối use case, quyền, dữ liệu từ repository.
    /// </summary>
    public sealed class UseCaseException : Exception
    {
        public UseCaseException(string message) : base(message)
        {
        }
    }
}
