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
    /// Tên model cho Trợ lý AI (ứng viên/NTD) và bóc tách JD — mặc định theo spec dự án.
    /// </summary>
    public string Model { get; set; } = "gemini-flash-latest";

    public int RequestTimeoutSeconds { get; set; } = 90;
}
