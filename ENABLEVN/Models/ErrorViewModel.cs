namespace Presentation.Models
{
    /// <summary>
    /// ViewModel mặc định cho trang lỗi.
    /// 
    /// Dùng bởi Views/Shared/Error.cshtml.
    /// </summary>
    public sealed class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
