using Domain.Candidates;
using Domain.Jobs;

namespace Application.Services;

/// <summary>
/// Tính điểm phù hợp giữa Candidate Profile và Job Post.
///
/// Công thức (theo plan):
///   Skill match       * 45%
///   Experience match  * 20%
///   Location match    * 10%
///   Salary match      * 10%
///   Position match    * 10%
///   Support type      *  5%
/// </summary>
public sealed class JobMatchScoringService : IJobMatchScoringService
{
    public JobMatchResult CalculateMatchScore(CandidateProfile candidate, JobPost job)
    {
        var candidateSkills = ParseTokens(candidate.Skills);
        var jobRequiredSkills = ParseTokens(job.Requirement);

        var (skillScore, matchedSkills, missingSkills) = CalculateSkillMatch(candidateSkills, jobRequiredSkills);
        var experienceScore = CalculateExperienceMatch(candidate.ExperienceSummary, job.Requirement);
        var salaryScore = CalculateSalaryMatch(candidate.DesiredSalary, job.SalaryRange);
        var positionScore = CalculatePositionMatch(candidate.DesiredPosition, job.Title.Value);
        var supportScore = CalculateSupportMatch(candidate.AccessibilityNeeds, job.AccessibilityInfo);

        // Location: cả 2 đều lưu free-text; không có cấu trúc nên dùng 50% làm baseline
        const double locationScore = 50.0;

        var total = skillScore * 0.45
                  + experienceScore * 0.20
                  + locationScore * 0.10
                  + salaryScore * 0.10
                  + positionScore * 0.10
                  + supportScore * 0.05;

        total = Math.Clamp(Math.Round(total, 1), 0, 100);

        return new JobMatchResult
        {
            Score = total,
            Level = ToLevel(total),
            Reason = BuildReason(total, matchedSkills, missingSkills),
            MatchedSkills = matchedSkills,
            MissingSkills = missingSkills
        };
    }

    // ────────── helpers ──────────

    private static (double score, List<string> matched, List<string> missing) CalculateSkillMatch(
        IReadOnlyList<string> candidateSkills,
        IReadOnlyList<string> jobSkills)
    {
        if (jobSkills.Count == 0)
            return (80.0, new List<string>(), new List<string>());

        if (candidateSkills.Count == 0)
            return (0.0, new List<string>(), jobSkills.ToList());

        var matched = new List<string>();
        var missing = new List<string>();

        foreach (var jSkill in jobSkills)
        {
            if (candidateSkills.Any(c => c.Contains(jSkill, StringComparison.OrdinalIgnoreCase)
                                      || jSkill.Contains(c, StringComparison.OrdinalIgnoreCase)))
                matched.Add(jSkill);
            else
                missing.Add(jSkill);
        }

        var score = (double)matched.Count / jobSkills.Count * 100.0;
        return (score, matched, missing);
    }

    private static double CalculateExperienceMatch(string? experienceSummary, string jobRequirement)
    {
        if (string.IsNullOrWhiteSpace(experienceSummary)) return 30.0;

        // Heuristic: nếu CV có nhiều từ trùng với yêu cầu JD thì điểm cao hơn
        var expTokens = ParseTokens(experienceSummary);
        var reqTokens = ParseTokens(jobRequirement);
        if (reqTokens.Count == 0) return 70.0;

        var overlap = expTokens.Count(e =>
            reqTokens.Any(r => r.Contains(e, StringComparison.OrdinalIgnoreCase)
                            || e.Contains(r, StringComparison.OrdinalIgnoreCase)));

        var ratio = (double)overlap / reqTokens.Count;
        return Math.Clamp(ratio * 100.0, 10.0, 100.0);
    }

    private static double CalculateSalaryMatch(decimal? desiredSalary, SalaryRange salary)
    {
        if (desiredSalary is null) return 50.0;
        if (salary.MinSalary is null && salary.MaxSalary is null) return 50.0;

        var min = salary.MinSalary ?? 0m;
        var max = salary.MaxSalary ?? decimal.MaxValue;

        if (desiredSalary >= min && desiredSalary <= max) return 100.0;

        // Candidate muốn cao hơn max
        if (desiredSalary > max && max > 0)
        {
            var diff = (double)((desiredSalary.Value - max) / max);
            return Math.Clamp((1.0 - diff) * 100.0, 0.0, 100.0);
        }

        return 70.0;
    }

    private static double CalculatePositionMatch(string? desiredPosition, string jobTitle)
    {
        if (string.IsNullOrWhiteSpace(desiredPosition)) return 40.0;

        var posTokens = ParseTokens(desiredPosition);
        var titleTokens = ParseTokens(jobTitle);
        if (titleTokens.Count == 0) return 40.0;

        var hit = posTokens.Any(p =>
            titleTokens.Any(t => t.Contains(p, StringComparison.OrdinalIgnoreCase)
                              || p.Contains(t, StringComparison.OrdinalIgnoreCase)));

        return hit ? 100.0 : 20.0;
    }

    private static double CalculateSupportMatch(string? candidateNeeds, JobAccessibilityInfo jobInfo)
    {
        if (string.IsNullOrWhiteSpace(candidateNeeds)) return 80.0;

        // Nếu job hỗ trợ accessibility thì điểm cao
        bool jobHasSupport = jobInfo.SupportsWheelchairAccess
                          || jobInfo.SupportsRemoteWork
                          || jobInfo.SupportsFlexibleTime
                          || jobInfo.ProvidesAssistiveDevices;

        return jobHasSupport ? 100.0 : 30.0;
    }

    private static List<string> ParseTokens(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return new List<string>();

        return text
            .Split(new[] { ',', ';', '\n', '\r', '|' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => t.Length >= 2)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static string ToLevel(double score) => score switch
    {
        >= 80 => "Phù hợp cao",
        >= 60 => "Cần xem xét",
        >= 40 => "Phù hợp trung bình",
        _ => "Chưa phù hợp"
    };

    private static string BuildReason(double score, List<string> matched, List<string> missing)
    {
        if (matched.Count == 0 && missing.Count == 0)
            return "Không đủ dữ liệu để đánh giá chi tiết.";

        if (score >= 80)
            return $"Ứng viên có nhiều kỹ năng trùng với yêu cầu công việc ({matched.Count} kỹ năng phù hợp).";

        if (score >= 60)
            return $"Ứng viên phù hợp một phần; còn thiếu {missing.Count} kỹ năng so với yêu cầu.";

        return $"Ứng viên thiếu nhiều kỹ năng yêu cầu ({missing.Count} kỹ năng chưa có).";
    }
}
