namespace Presentation.ViewModels.ApplicationChat;

public sealed class ApplicationChatInboxViewModel
{
    public IReadOnlyList<ApplicationChatInboxItemViewModel> Conversations { get; init; }
        = Array.Empty<ApplicationChatInboxItemViewModel>();
}
