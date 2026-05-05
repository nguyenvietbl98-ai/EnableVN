using Application.Common;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Outbound.Repositories;
using Presentation.ViewModels.ApplicationChat;

namespace Presentation.Controllers;

/// <summary>
/// Chat ứng viên ↔ NTD theo từng hồ sơ ứng tuyển (SignalR + lịch sử JSON).
/// </summary>
public sealed class ApplicationChatController : Controller
{
    private readonly IJobApplicationUseCase _jobApplicationUseCase;
    private readonly IApplicationChatRepository _chatRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IEmployerProfileRepository _employerProfileRepository;
    private readonly IJobApplicationRepository _jobApplicationRepository;

    public ApplicationChatController(
        IJobApplicationUseCase jobApplicationUseCase,
        IApplicationChatRepository chatRepository,
        IJobRepository jobRepository,
        IEmployerProfileRepository employerProfileRepository,
        IJobApplicationRepository jobApplicationRepository)
    {
        _jobApplicationUseCase = jobApplicationUseCase;
        _chatRepository = chatRepository;
        _jobRepository = jobRepository;
        _employerProfileRepository = employerProfileRepository;
        _jobApplicationRepository = jobApplicationRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Thread(Guid applicationId)
    {
        try
        {
            var info = await _jobApplicationUseCase.GetChatThreadForCurrentUserAsync(applicationId);
            return View(info);
        }
        catch (UseCaseException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> History(Guid applicationId)
    {
        try
        {
            await _jobApplicationUseCase.EnsureCurrentUserCanChatOnApplicationAsync(applicationId);

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!Guid.TryParse(userIdStr, out var currentUserId))
                return Unauthorized();

            var rows = await _chatRepository.ListByApplicationIdAsync(applicationId);

            return Json(
                new
                {
                    applicationId,
                    currentUserId,
                    messages = rows.Select(m => new
                    {
                        m.Id,
                        m.Body,
                        m.SenderUserId,
                        isMine = m.SenderUserId == currentUserId,
                        m.ModerationOutcome,
                        m.ModerationReasonVi,
                        sentAtUtc = m.SentAtUtc
                    })
                });
        }
        catch (UseCaseException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    [HttpGet]
    public async Task<IActionResult> Inbox()
    {
        try
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!Guid.TryParse(userIdStr, out var userId))
                return RedirectToAction("Login", "Auth");

            if (role == nameof(UserRole.Candidate))
            {
                var applications = await _jobApplicationUseCase.GetMyApplicationsAsync();
                return View(await BuildInboxViewModelFromApplicationResultsAsync(applications));
            }

            if (role == nameof(UserRole.Employer))
            {
                var profile = await _employerProfileRepository.GetByUserIdAsync(userId);
                if (profile is null)
                {
                    TempData["Error"] = "Bạn chưa có hồ sơ doanh nghiệp.";
                    return RedirectToAction("Index", "EmployerProfile");
                }

                var jobs = await _jobRepository.GetByEmployerIdAsync(profile.Id);
                var employerApplications = new List<Domain.Applications.JobApplication>();
                foreach (var job in jobs)
                {
                    var byJob = await _jobApplicationRepository.GetByJobIdAsync(job.Id);
                    employerApplications.AddRange(byJob.Where(a => a.Status != Domain.Applications.ApplicationStatus.Withdrawn));
                }

                var applicationResults = employerApplications
                    .Select(a => new Ports.Models.Applications.JobApplicationResult
                    {
                        Id = a.Id,
                        JobId = a.JobId,
                        CandidateId = a.CandidateId,
                        CoverLetter = a.CoverLetter,
                        CvUrl = a.CvUrl,
                        Status = a.Status,
                        SubmittedAt = a.SubmittedAt
                    })
                    .ToList();

                return View(await BuildInboxViewModelFromApplicationResultsAsync(applicationResults));
            }

            TempData["Error"] = "Vai trò hiện tại chưa hỗ trợ hộp thư chat.";
            return RedirectToAction("Index", "Home");
        }
        catch (UseCaseException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Home");
        }
    }

    private async Task<ApplicationChatInboxViewModel> BuildInboxViewModelFromApplicationResultsAsync(
        IReadOnlyCollection<Ports.Models.Applications.JobApplicationResult> applications)
    {
        var items = new List<ApplicationChatInboxItemViewModel>(applications.Count);

        foreach (var app in applications)
        {
            var messages = await _chatRepository.ListByApplicationIdAsync(app.Id);
            var lastMessage = messages.LastOrDefault();
            var job = await _jobRepository.GetByIdAsync(app.JobId);
            var fallbackTime = app.LatestHistoryAt ?? app.SubmittedAt;
            var lastActivity = lastMessage?.SentAtUtc ?? fallbackTime;
            var preview = lastMessage?.Body;
            if (string.IsNullOrWhiteSpace(preview))
            {
                preview = app.EmployerFeedbackNote;
            }

            if (string.IsNullOrWhiteSpace(preview))
            {
                preview = "Chưa có tin nhắn. Nhấn để bắt đầu trò chuyện.";
            }

            items.Add(new ApplicationChatInboxItemViewModel
            {
                ApplicationId = app.Id,
                JobId = app.JobId,
                JobTitle = job?.Title.ToString() ?? "Tin tuyển dụng",
                Status = app.Status,
                LastMessagePreview = preview,
                LastActivityUtc = lastActivity,
                SubmittedAtUtc = app.SubmittedAt,
                HasMessages = lastMessage is not null
            });
        }

        return new ApplicationChatInboxViewModel
        {
            Conversations = items
                .OrderByDescending(x => x.HasMessages)
                .ThenByDescending(x => x.LastActivityUtc)
                .ThenByDescending(x => x.SubmittedAtUtc)
                .ToList()
        };
    }
}
