using Application.Common;
using Application.Services;
using Domain.Jobs;
using Microsoft.Extensions.Logging;
using Ports.Inbound;
using Ports.Models.Jobs;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;

namespace Application.UseCases;

/// <summary>
/// Tính và trả danh sách "Việc làm phù hợp" cho Candidate đang đăng nhập.
/// Tái dụng IJobMatchScoringService để nhất quán với Match Score khi apply.
/// </summary>
public sealed class JobRecommendationUseCase : IJobRecommendationUseCase
{
    private readonly ICandidateProfileRepository _candidateProfileRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IEmployerProfileRepository _employerProfileRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IJobMatchScoringService _scoringService;
    private readonly ILogger<JobRecommendationUseCase> _logger;

    public JobRecommendationUseCase(
        ICandidateProfileRepository candidateProfileRepository,
        IJobRepository jobRepository,
        IJobApplicationRepository jobApplicationRepository,
        IEmployerProfileRepository employerProfileRepository,
        ICurrentUserService currentUser,
        IJobMatchScoringService scoringService,
        ILogger<JobRecommendationUseCase> logger)
    {
        _candidateProfileRepository = candidateProfileRepository;
        _jobRepository = jobRepository;
        _jobApplicationRepository = jobApplicationRepository;
        _employerProfileRepository = employerProfileRepository;
        _currentUser = currentUser;
        _scoringService = scoringService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<RecommendedJobDto>> GetRecommendedJobsAsync(
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = AuthorizationGuard.RequireCandidate(_currentUser);

        var candidateProfile = await _candidateProfileRepository.GetByUserIdAsync(userId, cancellationToken);
        if (candidateProfile is null)
        {
            _logger.LogWarning("Recommendation skipped: candidate profile not found. UserId={UserId}", userId);
            return Array.Empty<RecommendedJobDto>();
        }

        // Lấy tất cả active jobs
        var allJobs = await _jobRepository.SearchPublishedJobsAsync(
            keyword: null,
            workMode: null,
            supportsWheelchairAccess: null,
            supportsRemoteWork: null,
            supportsFlexibleTime: null,
            providesAssistiveDevices: null,
            cancellationToken);

        // Lịch sử ứng tuyển của candidate
        var myApplications = await _jobApplicationRepository.GetByCandidateIdAsync(
            candidateProfile.Id, cancellationToken);
        var appliedJobIds = myApplications.Select(a => a.JobId).ToHashSet();

        // Lấy công ty để hiển thị CompanyName
        var employerCache = new Dictionary<Guid, string>();

        var scored = new List<(RecommendedJobDto dto, double score)>();

        foreach (var job in allJobs)
        {
            try
            {
                var matchResult = _scoringService.CalculateMatchScore(candidateProfile, job);

                if (!employerCache.TryGetValue(job.EmployerId, out var companyName))
                {
                    var ep = await _employerProfileRepository.GetByIdAsync(job.EmployerId, cancellationToken);
                    companyName = ep?.CompanyName.Value ?? string.Empty;
                    employerCache[job.EmployerId] = companyName;
                }

                scored.Add((new RecommendedJobDto
                {
                    JobId = job.Id,
                    Title = job.Title.Value,
                    CompanyName = companyName,
                    Location = string.Empty,
                    SalaryMin = job.SalaryRange.MinSalary,
                    SalaryMax = job.SalaryRange.MaxSalary,
                    WorkMode = job.WorkMode.ToString(),
                    RecommendationScore = matchResult.Score,
                    MatchLevel = matchResult.Level,
                    MatchedSkills = matchResult.MatchedSkills,
                    MissingSkills = matchResult.MissingSkills,
                    AlreadyApplied = appliedJobIds.Contains(job.Id)
                }, matchResult.Score));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Score calculation failed for JobId={JobId}", job.Id);
            }
        }

        _logger.LogInformation(
            "Job recommendation calculated. CandidateId={CandidateId} TotalJobs={Total} Limit={Limit}",
            candidateProfile.Id, scored.Count, limit);

        return scored
            .OrderByDescending(x => x.score)
            .Take(limit)
            .Select(x => x.dto)
            .ToList();
    }
}
