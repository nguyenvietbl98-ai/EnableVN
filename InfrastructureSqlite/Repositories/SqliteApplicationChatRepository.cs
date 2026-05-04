using InfrastructureSqlite.Persistence;
using InfrastructureSqlite.PersistenceModels;
using Microsoft.EntityFrameworkCore;
using Ports.Models.Chat;
using Ports.Outbound.Repositories;

namespace InfrastructureSqlite.Repositories;

public sealed class SqliteApplicationChatRepository : IApplicationChatRepository
{
    private readonly EnableVnDbContext _db;

    public SqliteApplicationChatRepository(EnableVnDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(ApplicationChatMessageDto message, CancellationToken cancellationToken = default)
    {
        _db.ApplicationChatMessages.Add(
            new ApplicationChatMessageRecord
            {
                Id = message.Id,
                JobApplicationId = message.JobApplicationId,
                SenderUserId = message.SenderUserId,
                Body = message.Body,
                ModerationOutcome = message.ModerationOutcome,
                ModerationReasonVi = message.ModerationReasonVi,
                SentAtUtc = message.SentAtUtc
            });

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ApplicationChatMessageDto>> ListByApplicationIdAsync(
        Guid jobApplicationId,
        CancellationToken cancellationToken = default)
    {
        var rows = await _db.ApplicationChatMessages
            .AsNoTracking()
            .Where(x => x.JobApplicationId == jobApplicationId)
            .OrderBy(x => x.SentAtUtc)
            .ToListAsync(cancellationToken);

        return rows
            .Select(x => new ApplicationChatMessageDto
            {
                Id = x.Id,
                JobApplicationId = x.JobApplicationId,
                SenderUserId = x.SenderUserId,
                Body = x.Body,
                ModerationOutcome = x.ModerationOutcome,
                ModerationReasonVi = x.ModerationReasonVi,
                SentAtUtc = x.SentAtUtc
            })
            .ToList();
    }
}
