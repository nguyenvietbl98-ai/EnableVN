using Domain.Applications;

namespace Presentation.ViewModels.ApplicationChat;

public sealed class ApplicationChatInboxItemViewModel
{
    public Guid ApplicationId { get; init; }

    public Guid JobId { get; init; }

    public string JobTitle { get; init; } = string.Empty;

    public ApplicationStatus Status { get; init; }

    public string LastMessagePreview { get; init; } = string.Empty;

    public DateTime LastActivityUtc { get; init; }

    public DateTime SubmittedAtUtc { get; init; }

    public bool HasMessages { get; init; }
}
