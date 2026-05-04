namespace Presentation.ViewModels.Shared;

public sealed class BreadcrumbModel
{
    public string? MiddleText { get; init; }

    public string? MiddleController { get; init; }

    public string? MiddleAction { get; init; }

    public required string CurrentText { get; init; }
}
