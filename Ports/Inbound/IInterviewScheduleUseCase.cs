using Ports.Models.Interviews;

namespace Ports.Inbound;

public interface IInterviewScheduleUseCase
{
    Task<InterviewScheduleDto> CreateInterviewScheduleAsync(
        CreateInterviewScheduleRequest request,
        CancellationToken cancellationToken = default);

    Task AcceptInterviewAsync(Guid interviewScheduleId, CancellationToken cancellationToken = default);

    Task DeclineInterviewAsync(Guid interviewScheduleId, string? reason, CancellationToken cancellationToken = default);

    Task CancelInterviewAsync(Guid interviewScheduleId, string? reason, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InterviewScheduleDto>> GetMyInterviewsAsCandidateAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InterviewScheduleDto>> GetMyInterviewsAsEmployerAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InterviewScheduleDto>> GetByJobApplicationAsync(Guid jobApplicationId, CancellationToken cancellationToken = default);
}
