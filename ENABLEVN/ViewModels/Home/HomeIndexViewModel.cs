using Ports.Models.Jobs;

namespace Presentation.ViewModels.Home;

public sealed class HomeIndexViewModel
{
    public IReadOnlyList<JobResult> LatestJobs { get; init; } = [];
}
