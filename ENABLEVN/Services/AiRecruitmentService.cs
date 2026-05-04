using System.Text.Json;
using Domain.Jobs;
using Microsoft.AspNetCore.Http;
using Ports.Inbound;
using Ports.Models.Candidates;
using Ports.Models.Jobs;
using Presentation.Models.Ai;

namespace Presentation.Services;

public sealed class AiRecruitmentService
{
    private readonly GeminiClient _gemini;
    private readonly IJobUseCase _jobUseCase;
    private readonly IEmployerCandidateSearchUseCase _employerCandidateSearch;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private static readonly JsonSerializerOptions JsonRelaxed = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AiRecruitmentService(
        GeminiClient gemini,
        IJobUseCase jobUseCase,
        IEmployerCandidateSearchUseCase employerCandidateSearch,
        IHttpContextAccessor httpContextAccessor)
    {
        _gemini = gemini;
        _jobUseCase = jobUseCase;
        _employerCandidateSearch = employerCandidateSearch;
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsGeminiConfigured => _gemini.IsConfigured;

    public async Task<AiChatResponseDto> CandidateSuggestJobsAsync(string userMessage, CancellationToken ct)
    {
        var jobs = await _jobUseCase.SearchPublishedJobsAsync(new SearchJobQuery(), ct);
        var catalog = jobs
            .Take(60)
            .Select(j => new
            {
                id = j.Id,
                title = j.Title,
                workMode = j.WorkMode.ToString(),
                excerpt = j.Description.Length > 280 ? j.Description[..280] + "…" : j.Description
            })
            .ToList();

        var catalogJson = JsonSerializer.Serialize(catalog);
        var system = """
Bạn là trợ lý tuyển dụng hỗ trợ ứng viên EnableVN (nền tảng việc làm hòa nhập).
Chỉ được gợi ý tin từ danh sách JSON trong phần "CATALOG" — không bịa jobId.
Trả về ĐÚNG một JSON với schema:
{"assistantMessage":"string (tiếng Việt, ngắn gọn, thân thiện)","matches":[{"id":"guid","reason":"string"}]}
Tối đa 5 matches, sắp xếp theo độ phù hợp. Nếu không có tin phù hợp, matches là [].
""";

        var user = $"""
CATALOG (JSON array of jobs):
{catalogJson}

Ứng viên mô tả nhu cầu / kỹ năng / tình huống của họ (tiếng Việt):
{userMessage}
""";

        var raw = await _gemini.GenerateJsonTextAsync(system, user, ct);
        var env = JsonSerializer.Deserialize<GeminiChatEnvelope>(raw, JsonRelaxed)
                  ?? throw new InvalidOperationException("Không đọc được JSON từ Gemini.");

        return BuildJobResponse(jobs.Take(60).ToList(), env, userMessage);
    }

    public async Task<AiChatResponseDto> EmployerSuggestCandidatesAsync(string userMessage, CancellationToken ct)
    {
        var profiles = await _employerCandidateSearch.SearchAsync(new SearchPublicCandidatesQuery(), ct);
        var list = profiles.Take(60).ToList();

        var catalog = list.Select(p => new
        {
            id = p.Id,
            fullName = p.FullName,
            excerpt = string.IsNullOrWhiteSpace(p.Bio)
                ? ""
                : (p.Bio!.Length > 320 ? p.Bio[..320] + "…" : p.Bio)
        }).ToList();

        var catalogJson = JsonSerializer.Serialize(catalog);
        var system = """
Bạn là trợ lý tuyển dụng cho nhà tuyển dụng EnableVN.
Chỉ được gợi ý ứng viên từ CATALOG (JSON). id phải khớp chính xác một phần tử trong CATALOG.
Trả về ĐÚNG một JSON:
{"assistantMessage":"string (tiếng Việt)","matches":[{"id":"guid","reason":"string"}]}
Tối đa 5 matches. Nếu không phù hợp, matches là [].
""";

        var user = $"""
CATALOG:
{catalogJson}

Nhà tuyển dụng mô tả ứng viên họ cần tìm:
{userMessage}
""";

        var raw = await _gemini.GenerateJsonTextAsync(system, user, ct);
        var env = JsonSerializer.Deserialize<GeminiChatEnvelope>(raw, JsonRelaxed)
                  ?? throw new InvalidOperationException("Không đọc được JSON từ Gemini.");

        return BuildCandidateResponse(list, env);
    }

    public async Task<ParseJdResponseDto> ParseJobDescriptionAsync(string rawText, CancellationToken ct)
    {
        var system = """
Bạn phân tích mô tả công việc (JD) dán từ Word/text cho nền tảng EnableVN (tiếng Việt).
Trả về ĐÚNG một JSON với các khóa:
title, description, requirement (chuỗi; có thể nhiều đoạn),
workMode: một trong "Onsite","Remote","Hybrid",
minSalary, maxSalary (số VNĐ hoặc null nếu không rõ),
supportsWheelchairAccess, supportsRemoteWork, supportsFlexibleTime, providesAssistiveDevices (boolean, suy luận thận trọng từ JD),
additionalSupportDescription (string hoặc null),
biasWarnings: mảng các cảnh báo tiếng Việt nếu phát hiện nội dung phân biệt đối xử, xúc phạm, hoặc yêu cầu bất hợp pháp; nếu không có thì [].
Không thêm khóa ngoài schema.
""";

        var user = $"JD:\n{rawText}";
        var raw = await _gemini.GenerateJsonTextAsync(system, user, ct);
        var env = JsonSerializer.Deserialize<GeminiJdEnvelope>(raw, JsonRelaxed)
                  ?? throw new InvalidOperationException("Không đọc được JSON từ Gemini.");

        var wm = ParseWorkMode(env.WorkMode);

        return new ParseJdResponseDto
        {
            Title = (env.Title ?? "").Trim(),
            Description = (env.Description ?? "").Trim(),
            Requirement = (env.Requirement ?? "").Trim(),
            WorkMode = wm,
            MinSalary = env.MinSalary,
            MaxSalary = env.MaxSalary,
            SupportsWheelchairAccess = env.SupportsWheelchairAccess,
            SupportsRemoteWork = env.SupportsRemoteWork,
            SupportsFlexibleTime = env.SupportsFlexibleTime,
            ProvidesAssistiveDevices = env.ProvidesAssistiveDevices,
            AdditionalSupportDescription = string.IsNullOrWhiteSpace(env.AdditionalSupportDescription)
                ? null
                : env.AdditionalSupportDescription.Trim(),
            BiasWarnings = env.BiasWarnings ?? new List<string>()
        };
    }

    private AiChatResponseDto BuildJobResponse(IReadOnlyList<JobResult> jobs, GeminiChatEnvelope env, string userMessage)
    {
        _ = userMessage;
        var byId = jobs.ToDictionary(j => j.Id);
        var matches = new List<AiChatMatchDto>();
        foreach (var m in env.Matches ?? new List<GeminiChatMatch>())
        {
            if (m.Id is null || !Guid.TryParse(m.Id, out var gid) || !byId.TryGetValue(gid, out var job))
                continue;

            matches.Add(new AiChatMatchDto
            {
                Id = gid.ToString(),
                Title = job.Title,
                Reason = (m.Reason ?? "").Trim(),
                DetailUrl = BuildUrl($"/Jobs/Details/{gid}")
            });

            if (matches.Count >= 5)
                break;
        }

        return new AiChatResponseDto
        {
            AssistantMessage = string.IsNullOrWhiteSpace(env.AssistantMessage)
                ? "Dưới đây là một số gợi ý phù hợp."
                : env.AssistantMessage.Trim(),
            Matches = matches
        };
    }

    private AiChatResponseDto BuildCandidateResponse(IReadOnlyList<CandidateProfileResult> profiles, GeminiChatEnvelope env)
    {
        var byId = profiles.ToDictionary(p => p.Id);
        var matches = new List<AiChatMatchDto>();
        foreach (var m in env.Matches ?? new List<GeminiChatMatch>())
        {
            if (m.Id is null || !Guid.TryParse(m.Id, out var gid) || !byId.TryGetValue(gid, out var p))
                continue;

            matches.Add(new AiChatMatchDto
            {
                Id = gid.ToString(),
                Title = p.FullName,
                Reason = (m.Reason ?? "").Trim(),
                DetailUrl = BuildUrl($"/Candidates/Details/{gid}")
            });

            if (matches.Count >= 5)
                break;
        }

        return new AiChatResponseDto
        {
            AssistantMessage = string.IsNullOrWhiteSpace(env.AssistantMessage)
                ? "Dưới đây là một số ứng viên gợi ý."
                : env.AssistantMessage.Trim(),
            Matches = matches
        };
    }

    private string BuildUrl(string path)
    {
        var req = _httpContextAccessor.HttpContext?.Request;
        if (req is null)
            return path;

        var prefix = req.PathBase.HasValue ? req.PathBase.Value!.TrimEnd('/') : "";
        return string.IsNullOrEmpty(prefix) ? path : prefix + path;
    }

    private static string ParseWorkMode(string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return WorkMode.Remote.ToString();

        if (Enum.TryParse<WorkMode>(s.Trim(), true, out var w))
            return w.ToString();

        return WorkMode.Remote.ToString();
    }
}
