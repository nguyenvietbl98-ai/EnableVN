namespace Presentation.Options;

/// <summary>
/// Cấu hình Google Gemini (API key chỉ đặt trên server, không gửi xuống client).
/// </summary>
public sealed class GeminiOptions
{
    public const string SectionName = "Gemini";

    /// <summary>
    /// API key từ Google AI Studio / Cloud (ví dụ user secrets hoặc biến môi trường).
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Tên model, ví dụ: gemini-2.5-flash-lite, gemini-2.0-flash-lite, ...
    /// </summary>
    public string Model { get; set; } = "gemini-2.0-flash-lite";

    public int RequestTimeoutSeconds { get; set; } = 90;
}
