namespace Ports.Models.Chat;

/// <summary>
/// Một tin nhắn trong luồng chat gắn với hồ sơ ứng tuyển (ứng viên ↔ NTD).
/// </summary>
public sealed class ApplicationChatMessageDto
{
    public Guid Id { get; init; }

    public Guid JobApplicationId { get; init; }

    public Guid SenderUserId { get; init; }

    public string Body { get; init; } = string.Empty;

    /// <summary>allow | warn — tin bị block không được lưu.</summary>
    public string ModerationOutcome { get; init; } = "allow";

    public string? ModerationReasonVi { get; init; }

    public DateTime SentAtUtc { get; init; }
}
