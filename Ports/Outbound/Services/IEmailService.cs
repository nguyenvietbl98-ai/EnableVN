namespace Ports.Outbound.Services
{
    /// <summary>
    /// Outbound Port cho gửi email.
    /// 
    /// MVP có thể chưa dùng ngay.
    /// Giai đoạn 2 sẽ dùng để gửi thông báo khi trạng thái hồ sơ thay đổi.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Gửi email đơn giản (plain text).
        /// </summary>
        Task SendAsync(
            string to,
            string subject,
            string body,
            CancellationToken cancellationToken = default
        );
    }
}
