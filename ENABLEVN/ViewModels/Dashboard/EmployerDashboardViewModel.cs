namespace Presentation.ViewModels.Dashboard;

public sealed class EmployerDashboardViewModel
{
    public int TotalJobs { get; init; }

    public int OpenJobs { get; init; }

    public int TotalApplications { get; init; }

    public int UnreadNotifications { get; init; }
}
