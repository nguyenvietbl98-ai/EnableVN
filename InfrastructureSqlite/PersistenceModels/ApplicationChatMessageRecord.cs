namespace InfrastructureSqlite.PersistenceModels;

public sealed class ApplicationChatMessageRecord
{
    public Guid Id { get; set; }

    public Guid JobApplicationId { get; set; }

    public Guid SenderUserId { get; set; }

    public string Body { get; set; } = string.Empty;

    public string ModerationOutcome { get; set; } = "allow";

    public string? ModerationReasonVi { get; set; }

    public DateTime SentAtUtc { get; set; }
}
