using Ports.Models.Chat;

namespace Ports.Outbound.Repositories;

public interface IApplicationChatRepository
{
    Task AddAsync(ApplicationChatMessageDto message, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApplicationChatMessageDto>> ListByApplicationIdAsync(
        Guid jobApplicationId,
        CancellationToken cancellationToken = default);
}
