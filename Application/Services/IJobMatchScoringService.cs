using Domain.Candidates;
using Domain.Jobs;

namespace Application.Services;

/// <summary>
/// Outbound service tính điểm phù hợp giữa Candidate Profile và Job Post.
/// Dùng chung cho cả Match Score khi Apply và Job Recommendation.
/// </summary>
public interface IJobMatchScoringService
{
    JobMatchResult CalculateMatchScore(CandidateProfile candidate, JobPost job);
}

public sealed class JobMatchResult
{
    public double Score { get; init; }
    public string Level { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
    public List<string> MatchedSkills { get; init; } = new();
    public List<string> MissingSkills { get; init; } = new();
}
