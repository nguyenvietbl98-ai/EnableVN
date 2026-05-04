namespace Ports.Models.Chat;

/// <summary>
/// Thông tin hiển thị đầu trang chat (sau khi đã kiểm tra quyền).
/// </summary>
public sealed class ApplicationChatThreadDto
{
    public Guid ApplicationId { get; init; }

    public Guid JobId { get; init; }

    public string JobTitle { get; init; } = string.Empty;
}
