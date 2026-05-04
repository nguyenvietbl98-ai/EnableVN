using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Presentation.Options;

namespace Presentation.Services;

public enum ChatModerationAction
{
    Allow,
    Warn,
    Block
}

public sealed class ChatModerationResult
{
    public ChatModerationAction Action { get; init; }

    public string ReasonVi { get; init; } = string.Empty;
}

/// <summary>
/// Kiểm duyệt chat real-time (mục 3) — không dùng chung pipeline với <c>/api/ai/*</c> (Trợ lý + JD).
/// Cấu hình qua section <c>ChatModeration</c>; mặc định chỉ model ML zip (0 bình thường, 1 toxic).
/// </summary>
public sealed class ChatModerationService
{
    private readonly GeminiClient _gemini;
    private readonly ToxicCommentMlClassifier _ml;
    private readonly ChatModerationOptions _opt;

    public ChatModerationService(
        GeminiClient gemini,
        ToxicCommentMlClassifier ml,
        IOptions<ChatModerationOptions> options)
    {
        _gemini = gemini;
        _ml = ml;
        _opt = options.Value;
    }

    public async Task<ChatModerationResult> EvaluateAsync(string message, CancellationToken cancellationToken = default)
    {
        var text = message.Trim();
        var mode = (_opt.Mode ?? "ml").Trim().ToLowerInvariant();

        if (mode is "ml" or "ml_classifier" or "mlclassifier")
            return EvaluateMlOnly(text);

        if (mode is "gemini")
            return await EvaluateGeminiOnlyAsync(text, cancellationToken).ConfigureAwait(false);

        // ml_then_gemini (mặc định)
        if (_ml.IsConfigured && _ml.TryEvaluate(text, out var isToxic, out _))
        {
            if (isToxic)
            {
                return new ChatModerationResult
                {
                    Action = ChatModerationAction.Block,
                    ReasonVi =
                        "Nội dung bị phân loại là toxic (model TextClassifier). Vui lòng chỉnh sửa lịch sự hơn."
                };
            }
        }

        return await EvaluateGeminiOnlyAsync(text, cancellationToken).ConfigureAwait(false);
    }

    private ChatModerationResult EvaluateMlOnly(string text)
    {
        if (!_ml.IsConfigured)
        {
            throw new InvalidOperationException(
                "Chế độ chỉ ML nhưng chưa có file model hoặc đường dẫn sai. " +
                "Đặt ChatModeration:ClassifierModelPath trỏ tới TextClassifierModel.zip (vd: MLModels/TextClassifierModel.zip).");
        }

        if (!_ml.TryEvaluate(text, out var isToxic, out var err))
        {
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(err)
                    ? "Không chạy được model phân loại toxic."
                    : err);
        }

        if (isToxic)
        {
            return new ChatModerationResult
            {
                Action = ChatModerationAction.Block,
                ReasonVi =
                    "Nội dung bị phân loại là toxic (model TextClassifier, nhãn 1). Vui lòng chỉnh sửa."
            };
        }

        return new ChatModerationResult
        {
            Action = ChatModerationAction.Allow,
            ReasonVi = string.Empty
        };
    }

    private async Task<ChatModerationResult> EvaluateGeminiOnlyAsync(string text, CancellationToken cancellationToken)
    {
        if (!_gemini.IsConfigured)
        {
            throw new InvalidOperationException(
                "Chưa cấu hình Gemini (ApiKey/Model) trong khi bước kiểm duyệt cần Gemini. " +
                "Cấu hình Gemini hoặc đặt ChatModeration:Mode = ml (và cung cấp file model ML).");
        }

        var system =
            "Bạn là bộ lọc an toàn cho nền tảng tuyển dụng EnableVN (tiếng Việt). " +
            "Phân loại tin nhắn giữa ứng viên và nhà tuyển dụng.\n" +
            "Trả về DUY NHẤT một JSON hợp lệ với các khóa: action, reasonVi.\n" +
            "- action phải là một trong: allow, warn, block (chữ thường).\n" +
            "- allow: nội dung lịch sự, phù hợp môi trường tuyển dụng.\n" +
            "- warn: không nặng tới mức chặn nhưng có thể gây khó chịu, mập mờ, hoặc hơi thiếu chuyên nghiệp — vẫn cho gửi kèm cảnh báo.\n" +
            "- block: tục tĩu, quấy rối, phân biệt đối xử, thù hận, đe dọa, spam lừa đảo, hoặc nội dung không phù hợp nghiêm trọng.\n" +
            "- reasonVi: giải thích ngắn bằng tiếng Việt (tối đa 240 ký tự), rỗng nếu allow.\n" +
            "Không thêm khóa khác. Không bọc trong markdown.";

        var raw = await _gemini.GenerateJsonTextAsync(system, text, cancellationToken).ConfigureAwait(false);

        ModerationJson? parsed = null;
        try
        {
            parsed = JsonSerializer.Deserialize<ModerationJson>(raw, JsonOptions);
        }
        catch
        {
            // ignore
        }

        var actionStr = parsed?.Action?.Trim().ToLowerInvariant() ?? string.Empty;
        var action = actionStr switch
        {
            "block" => ChatModerationAction.Block,
            "warn" => ChatModerationAction.Warn,
            "allow" => ChatModerationAction.Allow,
            _ => ChatModerationAction.Allow
        };

        var reason = (parsed?.ReasonVi ?? string.Empty).Trim();
        if (reason.Length > 500)
            reason = reason[..500];

        return new ChatModerationResult
        {
            Action = action,
            ReasonVi = reason
        };
    }

    private sealed class ModerationJson
    {
        [JsonPropertyName("action")]
        public string? Action { get; set; }

        [JsonPropertyName("reasonVi")]
        public string? ReasonVi { get; set; }
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
