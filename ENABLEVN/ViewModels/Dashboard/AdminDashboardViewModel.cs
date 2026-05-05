namespace Presentation.ViewModels.Dashboard;

public sealed class AdminDashboardViewModel
{
    public int PendingReports { get; init; }

    public int UnreadNotifications { get; init; }

    public IReadOnlyList<Ports.Models.Employers.EmployerProfileReviewItemResult> PendingEmployerProfiles { get; init; }
        = Array.Empty<Ports.Models.Employers.EmployerProfileReviewItemResult>();

    public IReadOnlyList<Ports.Models.Employers.EmployerProfileReviewItemResult> EmployerProfilesForReview { get; init; }
        = Array.Empty<Ports.Models.Employers.EmployerProfileReviewItemResult>();
}
