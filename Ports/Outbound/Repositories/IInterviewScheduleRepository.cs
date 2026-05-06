using Domain.Interviews;

namespace Ports.Outbound.Repositories;

public interface IInterviewScheduleRepository
{
    Task AddAsync(InterviewSchedule schedule, CancellationToken cancellationToken = default);

    Task<InterviewSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InterviewSchedule>> GetByJobApplicationIdAsync(
        Guid jobApplicationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InterviewSchedule>> GetByCandidateUserIdAsync(
        Guid candidateUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InterviewSchedule>> GetByEmployerUserIdAsync(
        Guid employerUserId,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(InterviewSchedule schedule, CancellationToken cancellationToken = default);
}
