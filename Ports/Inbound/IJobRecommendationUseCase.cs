using Ports.Models.Jobs;

namespace Ports.Inbound;

public interface IJobRecommendationUseCase
{
    /// <summary>
    /// Trả danh sách job phù hợp với candidate hiện tại (đang đăng nhập).
    /// Sort theo RecommendationScore giảm dần.
    /// </summary>
    Task<IReadOnlyList<RecommendedJobDto>> GetRecommendedJobsAsync(
        int limit = 10,
        CancellationToken cancellationToken = default);
}
