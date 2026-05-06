namespace Ports.Models.Jobs;

public sealed class RecommendedJobDto
{
    public Guid JobId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public decimal? SalaryMin { get; init; }
    public decimal? SalaryMax { get; init; }
    public string WorkMode { get; init; } = string.Empty;
    public double RecommendationScore { get; init; }
    public string MatchLevel { get; init; } = string.Empty;
    public List<string> MatchedSkills { get; init; } = new();
    public List<string> MissingSkills { get; init; } = new();
    public bool AlreadyApplied { get; init; }
}
