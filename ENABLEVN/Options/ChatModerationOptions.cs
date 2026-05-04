namespace Presentation.Options;

/// <summary>
/// Kiểm duyệt chat: ML (TextClassifierModel.zip: 0 bình thường, 1 toxic) và/hoặc Gemini.
/// </summary>
public sealed class ChatModerationOptions
{
    public const string SectionName = "ChatModeration";

    /// <summary>
    /// ml — chỉ file ML (tách khỏi Gemini dùng cho mục 1–2). gemini — chỉ Gemini. ml_then_gemini — ML chặn toxic trước, còn lại Gemini.
    /// </summary>
    public string Mode { get; set; } = "ml";

    /// <summary>
    /// Đường dẫn tới TextClassifierModel.zip: tuyệt đối, hoặc tương đối so với thư mục project web (ContentRoot).
    /// Ví dụ: MLModels/TextClassifierModel.zip hoặc ../Application/bin/Debug/net10.0/TextClassifierModel.zip
    /// </summary>
    public string ClassifierModelPath { get; set; } = "MLModels/TextClassifierModel.zip";

    /// <summary>Tên cột văn bản trong pipeline train (phải khớp model.zip). Lỗi "Could not find input column 'X'" → đặt đúng X.</summary>
    public string InputColumnName { get; set; } = "TextContent";

    /// <summary>Nhãn toxic trong PredictedLabel (thường là 1).</summary>
    public float ToxicLabelValue { get; set; } = 1f;

    /// <summary>Chỉ chặn khi cột <c>Probability</c> (xác suất toxic) &gt;= giá trị này [0–1]. Mặc định 0,75 — dưới ngưỡng cho phép.</summary>
    public float ToxicProbabilityThreshold { get; set; } = 0.75f;
}
