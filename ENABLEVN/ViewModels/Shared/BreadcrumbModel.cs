using System.Collections.Generic;

namespace Presentation.ViewModels.Shared;

public sealed class BreadcrumbModel
{
    public string? MiddleText { get; init; }

    public string? MiddleController { get; init; }

    public string? MiddleAction { get; init; }

    /// <summary>Tham số bổ sung cho liên kết giữa (vd: jobId cho ByJob).</summary>
    public Dictionary<string, string>? MiddleRoute { get; init; }

    public required string CurrentText { get; init; }
}
