using Microsoft.Extensions.Options;
using Microsoft.ML;
using Microsoft.ML.Data;
using Presentation.Options;

namespace Presentation.Services;

/// <summary>
/// Tải model zip ML.NET (cột đầu vào <c>TextContent</c>, nhãn toxic trong <c>PredictedLabel</c> / <c>Prediction</c>).
/// </summary>
public sealed class ToxicCommentMlClassifier : IDisposable
{
    private readonly ILogger<ToxicCommentMlClassifier> _logger;
    private readonly ChatModerationOptions _opt;
    private readonly string? _resolvedPath;

    private readonly object _gate = new();
    private MLContext? _ml;
    private ITransformer? _model;
    private PredictionEngine<TextClassifierRow, TextClassifierPrediction>? _engine;
    private bool _loadAttempted;
    private string? _loadError;

    public ToxicCommentMlClassifier(
        IOptions<ChatModerationOptions> options,
        IWebHostEnvironment env,
        ILogger<ToxicCommentMlClassifier> logger)
    {
        _logger = logger;
        _opt = options.Value;

        var raw = (_opt.ClassifierModelPath ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(raw))
        {
            _resolvedPath = null;
            return;
        }

        _resolvedPath = Path.IsPathRooted(raw)
            ? raw
            : Path.GetFullPath(Path.Combine(env.ContentRootPath, raw));

        if (!File.Exists(_resolvedPath))
        {
            _logger.LogWarning(
                "Chat ML: không thấy file model tại {Path} (đã resolve từ cấu hình).",
                _resolvedPath);
            _resolvedPath = null;
        }
    }

    public bool IsConfigured => _resolvedPath is not null;

    public bool TryEvaluate(string message, out bool isToxic, out string? error)
    {
        isToxic = false;
        error = null;

        if (_resolvedPath is null)
            return false;

        EnsureEngine();
        if (_engine is null)
        {
            error = _loadError ?? "Không tải được model ML.";
            return false;
        }

        try
        {
            var row = new TextClassifierRow { TextContent = message };
            var pred = _engine.Predict(row);
            isToxic = IsToxicPrediction(pred);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Chat ML: lỗi khi predict.");
            error = "Lỗi khi chạy model phân loại.";
            return false;
        }
    }

    private bool IsToxicPrediction(TextClassifierPrediction pred)
    {
        // Chỉ chặn khi xác suất toxic đủ cao; boolean Prediction không dùng để tránh quá khắt.
        return pred.Probability >= _opt.ToxicProbabilityThreshold;
    }

    private void EnsureEngine()
    {
        if (_loadAttempted)
            return;

        lock (_gate)
        {
            if (_loadAttempted)
                return;

            _loadAttempted = true;
            if (_resolvedPath is null)
                return;

            try
            {
                _ml = new MLContext(seed: 0);
                _model = _ml.Model.Load(_resolvedPath, out _);

                var configuredInput = string.IsNullOrWhiteSpace(_opt.InputColumnName)
                    ? "TextContent"
                    : _opt.InputColumnName.Trim();

                // Tên property là TextContent; chỉ bọc SchemaDefinition nếu tên cột trong model khác.
                if (!string.Equals(configuredInput, nameof(TextClassifierRow.TextContent), StringComparison.Ordinal))
                {
                    var inputSchema = SchemaDefinition.Create(typeof(TextClassifierRow));
                    inputSchema[nameof(TextClassifierRow.TextContent)].ColumnName = configuredInput;
                    _engine = _ml.Model.CreatePredictionEngine<TextClassifierRow, TextClassifierPrediction>(
                        _model,
                        ignoreMissingColumns: true,
                        inputSchemaDefinition: inputSchema);
                }
                else
                {
                    _engine = _ml.Model.CreatePredictionEngine<TextClassifierRow, TextClassifierPrediction>(
                        _model,
                        ignoreMissingColumns: true);
                }

                _logger.LogInformation("Chat ML: đã tải model từ {Path}.", _resolvedPath);
            }
            catch (Exception ex)
            {
                _loadError = ex.Message;
                _logger.LogError(ex, "Chat ML: không tải được model từ {Path}.", _resolvedPath);
                _model = null;
                _engine = null;
            }
        }
    }

    public void Dispose()
    {
        _engine?.Dispose();
        _engine = null;
        _model = null;
        _ml = null;
    }

    /// <summary>Khớp [LoadColumn(0)] khi train.</summary>
    private sealed class TextClassifierRow
    {
        public string TextContent { get; set; } = "";
    }

    /// <summary>Khớp output khi train (PredictedLabel, Probability, Score dạng scalar).</summary>
    private sealed class TextClassifierPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        [ColumnName("Probability")]
        public float Probability { get; set; }

        [ColumnName("Score")]
        public float Score { get; set; }
    }
}
