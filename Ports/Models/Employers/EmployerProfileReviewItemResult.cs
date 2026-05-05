namespace Ports.Models.Employers;

public sealed class EmployerProfileReviewItemResult
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public string CompanyName { get; init; } = string.Empty;

    public string? Industry { get; init; }

    public string? ContactEmail { get; init; }

    public string? PhoneNumber { get; init; }

    public string VerificationStatus { get; init; } = string.Empty;

    public DateTime? VerifiedAtUtc { get; init; }

    public string? VerificationNote { get; init; }
}
