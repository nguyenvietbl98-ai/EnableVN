using System.Text.Json.Serialization;

namespace Presentation.Models.Ai;


public sealed class AiChatRequest
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = "";
}

public sealed class AiChatMatchDto
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Reason { get; set; } = "";
    public string DetailUrl { get; set; } = "";
}

public sealed class AiChatResponseDto
{
    public string AssistantMessage { get; set; } = "";
    public IReadOnlyList<AiChatMatchDto> Matches { get; set; } = Array.Empty<AiChatMatchDto>();
}

public sealed class ParseJdRequest
{
    [JsonPropertyName("rawText")]
    public string RawText { get; set; } = "";
}

public sealed class ParseJdResponseDto
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Requirement { get; set; } = "";
    public string WorkMode { get; set; } = "Remote";
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public bool SupportsWheelchairAccess { get; set; }
    public bool SupportsRemoteWork { get; set; }
    public bool SupportsFlexibleTime { get; set; }
    public bool ProvidesAssistiveDevices { get; set; }
    public string? AdditionalSupportDescription { get; set; }
    public IReadOnlyList<string> BiasWarnings { get; set; } = Array.Empty<string>();
}

/// <summary>
/// JSON schema Gemini trả về cho chat (nội bộ deserialize).
/// </summary>
internal sealed class GeminiChatEnvelope
{
    [JsonPropertyName("assistantMessage")]
    public string AssistantMessage { get; set; } = "";

    [JsonPropertyName("matches")]
    public List<GeminiChatMatch> Matches { get; set; } = [];
}

internal sealed class GeminiChatMatch
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

internal sealed class GeminiJdEnvelope
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("requirement")]
    public string? Requirement { get; set; }

    [JsonPropertyName("workMode")]
    public string? WorkMode { get; set; }

    [JsonPropertyName("minSalary")]
    public decimal? MinSalary { get; set; }

    [JsonPropertyName("maxSalary")]
    public decimal? MaxSalary { get; set; }

    [JsonPropertyName("supportsWheelchairAccess")]
    public bool SupportsWheelchairAccess { get; set; }

    [JsonPropertyName("supportsRemoteWork")]
    public bool SupportsRemoteWork { get; set; }

    [JsonPropertyName("supportsFlexibleTime")]
    public bool SupportsFlexibleTime { get; set; }

    [JsonPropertyName("providesAssistiveDevices")]
    public bool ProvidesAssistiveDevices { get; set; }

    [JsonPropertyName("additionalSupportDescription")]
    public string? AdditionalSupportDescription { get; set; }

    [JsonPropertyName("biasWarnings")]
    public List<string>? BiasWarnings { get; set; }
}
