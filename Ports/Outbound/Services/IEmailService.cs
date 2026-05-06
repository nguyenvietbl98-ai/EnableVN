namespace Ports.Outbound.Services
{
    /// <summary>
    /// Outbound Port cho gửi email.
    /// 
     /// Body được truyền vào dưới dạng HTML (UTF-8) để hỗ trợ email tiếng Việt & template.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Gửi email (HTML).
        /// </summary>
        Task SendAsync(
            string to,
            string subject,
            string body,
            CancellationToken cancellationToken = default
        );
    }
}
