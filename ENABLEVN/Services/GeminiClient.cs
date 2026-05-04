using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Presentation.Options;

namespace Presentation.Services;

/// <summary>
/// Gọi Gemini generateContent (REST v1beta). API key không bao giờ trả về client.
/// </summary>
public sealed class GeminiClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _model;

    public GeminiClient(HttpClient http, IOptions<GeminiOptions> options)
    {
        _http = http;
        var o = options.Value;
        _apiKey = (o.ApiKey ?? string.Empty).Trim();
        _model = (o.Model ?? string.Empty).Trim();
    }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_apiKey) && !string.IsNullOrWhiteSpace(_model);

    public async Task<string> GenerateJsonTextAsync(
        string systemInstruction,
        string userText,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
            throw new InvalidOperationException("Chưa cấu hình Gemini:ApiKey hoặc Gemini:Model trong cấu hình ứng dụng.");

        // Gộp system + user vào một turn "user" — tương thích tốt hơn với mọi model / endpoint.
        var merged =
            "[HỆ THỐNG — tuân thủ tuyệt đối]\n" +
            systemInstruction.Trim() +
            "\n\n[NỘI DUNG NGƯỜI DÙNG]\n" +
            userText.Trim();

        Exception? last = null;
        foreach (var useJsonMime in new[] { true, false })
        {
            try
            {
                return await SendOnceAsync(merged, useJsonMime, cancellationToken);
            }
            catch (Exception ex)
            {
                last = ex;
            }
        }

        throw new InvalidOperationException(
            last?.Message ?? "Không gọi được Gemini sau nhiều lần thử.");
    }

    private async Task<string> SendOnceAsync(string mergedText, bool jsonMime, CancellationToken cancellationToken)
    {
        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{Uri.EscapeDataString(_model)}:generateContent?key={Uri.EscapeDataString(_apiKey)}";

        var generationConfig = new Dictionary<string, object?>
        {
            ["temperature"] = 0.25
        };
        if (jsonMime)
            generationConfig["responseMimeType"] = "application/json";

        var payload = new Dictionary<string, object?>
        {
            ["contents"] = new object[]
            {
                new Dictionary<string, object?>
                {
                    ["role"] = "user",
                    ["parts"] = new object[] { new Dictionary<string, string> { ["text"] = mergedText } }
                }
            },
            ["generationConfig"] = generationConfig
        };

        var json = JsonSerializer.Serialize(payload, SerializeOptions);
        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var resp = await _http.SendAsync(req, cancellationToken);
        var body = await resp.Content.ReadAsStringAsync(cancellationToken);

        if (!resp.IsSuccessStatusCode)
            throw new InvalidOperationException(FormatHttpError(resp.StatusCode, body));

        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        if (root.TryGetProperty("error", out var errEl))
        {
            var msg = errEl.TryGetProperty("message", out var m) ? m.GetString() : errEl.ToString();
            throw new InvalidOperationException($"Gemini API: {msg}");
        }

        if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            throw new InvalidOperationException($"Gemini không trả candidates: {Truncate(body, 400)}");

        var first = candidates[0];
        if (first.TryGetProperty("finishReason", out var fr))
        {
            var reason = fr.GetString();
            if (string.Equals(reason, "SAFETY", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(reason, "BLOCKLIST", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Gemini chặn nội dung theo chính sách an toàn. Hãy thử chỉnh lại mô tả.");
        }

        if (!first.TryGetProperty("content", out var content) ||
            !content.TryGetProperty("parts", out var parts) ||
            parts.GetArrayLength() == 0)
            throw new InvalidOperationException("Gemini trả về không có nội dung (parts rỗng). Thử model khác hoặc rút gọn JD.");

        var text = parts[0].TryGetProperty("text", out var tEl) ? tEl.GetString() : null;
        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException("Gemini trả về nội dung rỗng.");

        return UnwrapJsonFence(text);
    }

    private static string FormatHttpError(System.Net.HttpStatusCode code, string body)
    {
        try
        {
            using var d = JsonDocument.Parse(body);
            if (d.RootElement.TryGetProperty("error", out var e) &&
                e.TryGetProperty("message", out var m))
                return $"Gemini HTTP {(int)code}: {m.GetString()}";
        }
        catch
        {
            // ignore
        }

        return $"Gemini HTTP {(int)code}: {Truncate(body, 500)}";
    }

    private static string UnwrapJsonFence(string text)
    {
        var t = text.Trim();
        if (t.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
            t = t["```json".Length..].Trim();
        else if (t.StartsWith("```", StringComparison.Ordinal))
            t = t[3..].Trim();

        if (t.EndsWith("```", StringComparison.Ordinal))
            t = t[..^3].Trim();

        return t;
    }

    private static string Truncate(string s, int max) =>
        s.Length <= max ? s : s[..max] + "…";

    private static readonly JsonSerializerOptions SerializeOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
