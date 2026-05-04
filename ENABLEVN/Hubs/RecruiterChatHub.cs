using Microsoft.AspNetCore.SignalR;
using Ports.Inbound;
using Ports.Models.Chat;
using Ports.Outbound.Repositories;
using Presentation.Services;

namespace Presentation.Hubs;

/// <summary>
/// Chat real-time ứng viên ↔ NTD (mục 3). Kiểm duyệt qua <c>ChatModerationService</c> — tách khỏi <c>/api/ai/*</c> (Trợ lý + JD, mục 1–2).
/// </summary>
public sealed class RecruiterChatHub : Hub
{
    public const string HubPath = "/hubs/recruiter-chat";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RecruiterChatHub> _logger;

    public RecruiterChatHub(IServiceScopeFactory scopeFactory, ILogger<RecruiterChatHub> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public static string ThreadGroupName(Guid applicationId) => $"appchat:{applicationId:N}";

    public async Task JoinThread(Guid applicationId)
    {
        using var scope = _scopeFactory.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<IJobApplicationUseCase>();

        await useCase.EnsureCurrentUserCanChatOnApplicationAsync(
            applicationId,
            Context.ConnectionAborted);

        await Groups.AddToGroupAsync(Context.ConnectionId, ThreadGroupName(applicationId));
    }

    public async Task SendMessage(Guid applicationId, string text)
    {
        var trimmed = (text ?? string.Empty).Trim();
        if (trimmed.Length == 0)
            throw new HubException("Tin nhắn trống.");

        if (trimmed.Length > 4000)
            throw new HubException("Tin nhắn quá dài (tối đa 4000 ký tự).");

        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;
        var useCase = sp.GetRequiredService<IJobApplicationUseCase>();
        var moderation = sp.GetRequiredService<ChatModerationService>();
        var repo = sp.GetRequiredService<IApplicationChatRepository>();
        var httpAccessor = sp.GetRequiredService<IHttpContextAccessor>();
        var userId = SessionUserIdOrEmpty(httpAccessor.HttpContext);
        if (userId == Guid.Empty)
            throw new HubException("Phiên đăng nhập không hợp lệ — hãy tải lại trang.");

        await useCase.EnsureCurrentUserCanChatOnApplicationAsync(applicationId, Context.ConnectionAborted);

        ChatModerationResult decision;
        try
        {
            decision = await moderation.EvaluateAsync(trimmed, Context.ConnectionAborted);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Chat moderation unavailable");
            throw new HubException(ex.Message);
        }

        if (decision.Action == ChatModerationAction.Block)
        {
            await Clients.Caller.SendAsync(
                "MessageBlocked",
                new
                {
                    reasonVi = string.IsNullOrWhiteSpace(decision.ReasonVi)
                        ? "Nội dung không được phép gửi trên EnableVN."
                        : decision.ReasonVi
                },
                cancellationToken: Context.ConnectionAborted);
            return;
        }

        var outcome = decision.Action == ChatModerationAction.Warn ? "warn" : "allow";
        var reasonVi = decision.Action == ChatModerationAction.Warn ? decision.ReasonVi : null;

        var dto = new ApplicationChatMessageDto
        {
            Id = Guid.NewGuid(),
            JobApplicationId = applicationId,
            SenderUserId = userId,
            Body = trimmed,
            ModerationOutcome = outcome,
            ModerationReasonVi = string.IsNullOrWhiteSpace(reasonVi) ? null : reasonVi,
            SentAtUtc = DateTime.UtcNow
        };

        await repo.AddAsync(dto, Context.ConnectionAborted);

        var payload = new
        {
            dto.Id,
            dto.JobApplicationId,
            dto.SenderUserId,
            dto.Body,
            dto.ModerationOutcome,
            dto.ModerationReasonVi,
            dto.SentAtUtc
        };

        await Clients.Group(ThreadGroupName(applicationId)).SendAsync("ReceiveMessage", payload, Context.ConnectionAborted);
    }

    private static Guid SessionUserIdOrEmpty(HttpContext? http)
    {
        if (http?.Session is null)
            return Guid.Empty;

        var s = http.Session.GetString("UserId");
        return Guid.TryParse(s, out var id) ? id : Guid.Empty;
    }
}
