using Domain.Interviews;
using InfrastructureSqlite.Mappers;
using InfrastructureSqlite.Persistence;
using Microsoft.EntityFrameworkCore;
using Ports.Outbound.Repositories;

namespace InfrastructureSqlite.Repositories;

public sealed class SqliteInterviewScheduleRepository : IInterviewScheduleRepository
{
    private readonly EnableVnDbContext _db;

    public SqliteInterviewScheduleRepository(EnableVnDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(InterviewSchedule schedule, CancellationToken cancellationToken = default)
    {
        var record = InterviewSchedulePersistenceMapper.ToRecord(schedule);
        await _db.InterviewSchedules.AddAsync(record, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<InterviewSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var record = await _db.InterviewSchedules
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return record is null ? null : InterviewSchedulePersistenceMapper.ToDomain(record);
    }

    public async Task<IReadOnlyList<InterviewSchedule>> GetByJobApplicationIdAsync(
        Guid jobApplicationId, CancellationToken cancellationToken = default)
    {
        var records = await _db.InterviewSchedules
            .AsNoTracking()
            .Where(x => x.JobApplicationId == jobApplicationId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return records.Select(InterviewSchedulePersistenceMapper.ToDomain).ToList();
    }

    public async Task<IReadOnlyList<InterviewSchedule>> GetByCandidateUserIdAsync(
        Guid candidateUserId, CancellationToken cancellationToken = default)
    {
        var records = await _db.InterviewSchedules
            .AsNoTracking()
            .Where(x => x.CandidateUserId == candidateUserId)
            .OrderByDescending(x => x.ScheduledAt)
            .ToListAsync(cancellationToken);

        return records.Select(InterviewSchedulePersistenceMapper.ToDomain).ToList();
    }

    public async Task<IReadOnlyList<InterviewSchedule>> GetByEmployerUserIdAsync(
        Guid employerUserId, CancellationToken cancellationToken = default)
    {
        var records = await _db.InterviewSchedules
            .AsNoTracking()
            .Where(x => x.EmployerUserId == employerUserId)
            .OrderByDescending(x => x.ScheduledAt)
            .ToListAsync(cancellationToken);

        return records.Select(InterviewSchedulePersistenceMapper.ToDomain).ToList();
    }

    public async Task UpdateAsync(InterviewSchedule schedule, CancellationToken cancellationToken = default)
    {
        var record = await _db.InterviewSchedules
            .FirstOrDefaultAsync(x => x.Id == schedule.Id, cancellationToken)
            ?? throw new InvalidOperationException("Không tìm thấy InterviewSchedule để cập nhật.");

        InterviewSchedulePersistenceMapper.UpdateRecord(record, schedule);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
