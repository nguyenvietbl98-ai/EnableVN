using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Outbound.Services
{
    /// <summary>
    /// Outbound Port cho lưu trữ file.
    /// 
    /// Dùng cho CV của Candidate, logo công ty, tài liệu đính kèm.
    /// 
    /// Infrastructure có thể implement bằng:
    /// - Local folder
    /// - SQLite blob
    /// - S3
    /// - Cloudinary
    /// - Azure Blob Storage
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Lưu file và trả về URL hoặc path để hệ thống sử dụng.
        /// </summary>
        Task<string> SaveAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Xóa file theo URL hoặc path.
        /// </summary>
        Task DeleteAsync(
            string fileUrl,
            CancellationToken cancellationToken = default
        );
    }
}
