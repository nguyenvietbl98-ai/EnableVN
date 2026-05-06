using Domain.Applications;
using System.Net;
using System.Text;

namespace Application.Email;

public static class EmailTemplates
{
    public static string RenderWelcomeEmailHtml(string recipientEmail)
    {
        var safeEmail = WebUtility.HtmlEncode(recipientEmail);

        var body = $@"
            <h2 style=""margin:0 0 12px 0;font-size:20px;line-height:28px;color:#111827;"">Chào mừng bạn đến với EnableVN</h2>
            <p style=""margin:0 0 12px 0;color:#374151;font-size:14px;line-height:22px;"">
                Xin chào <strong>{safeEmail}</strong>,
            </p>
            <p style=""margin:0 0 12px 0;color:#374151;font-size:14px;line-height:22px;"">
                Tài khoản của bạn đã được tạo thành công. EnableVN rất vui được đồng hành cùng bạn trên hành trình tìm kiếm cơ hội phù hợp.
            </p>
            <p style=""margin:0;color:#6B7280;font-size:13px;line-height:20px;"">
                Nếu bạn không thực hiện thao tác này, vui lòng bỏ qua email.
            </p>";

        return RenderLayoutHtml(
            title: "Chào mừng bạn đến với EnableVN",
            preheader: "Tài khoản EnableVN của bạn đã được tạo thành công.",
            bodyHtml: body);
    }

    public static string RenderEmployerProfileReviewedHtml(
        string companyName,
        bool approved,
        string? reasonOrNote)
    {
        var safeCompany = WebUtility.HtmlEncode(companyName);
        var statusText = approved ? "ĐÃ ĐƯỢC DUYỆT" : "ĐÃ BỊ TỪ CHỐI";
        var statusColor = approved ? "#065F46" : "#991B1B";
        var statusBg = approved ? "#D1FAE5" : "#FEE2E2";

        var noteBlock = string.IsNullOrWhiteSpace(reasonOrNote)
            ? string.Empty
            : $@"
                <div style=""margin-top:12px;padding:12px;border-radius:10px;background:#F9FAFB;border:1px solid #E5E7EB;"">
                  <div style=""font-size:12px;color:#6B7280;margin-bottom:6px;"">Ghi chú từ quản trị viên</div>
                  <div style=""font-size:14px;line-height:22px;color:#111827;"">{WebUtility.HtmlEncode(reasonOrNote)}</div>
                </div>";

        var body = $@"
            <h2 style=""margin:0 0 12px 0;font-size:20px;line-height:28px;color:#111827;"">Kết quả xét duyệt hồ sơ doanh nghiệp</h2>
            <p style=""margin:0 0 12px 0;color:#374151;font-size:14px;line-height:22px;"">
                Hồ sơ doanh nghiệp <strong>{safeCompany}</strong> { (approved ? "đã" : "đã") } được cập nhật trạng thái.
            </p>
            <div style=""display:inline-block;padding:6px 10px;border-radius:999px;background:{statusBg};color:{statusColor};font-weight:700;font-size:12px;letter-spacing:.3px;"">
                {statusText}
            </div>
            {noteBlock}
            <p style=""margin:16px 0 0 0;color:#6B7280;font-size:13px;line-height:20px;"">
                Bạn có thể đăng nhập EnableVN để xem chi tiết và cập nhật hồ sơ nếu cần.
            </p>";

        return RenderLayoutHtml(
            title: "Kết quả xét duyệt hồ sơ doanh nghiệp",
            preheader: approved ? "Hồ sơ doanh nghiệp của bạn đã được duyệt." : "Hồ sơ doanh nghiệp của bạn đã bị từ chối.",
            bodyHtml: body);
    }

    public static string RenderApplicationStatusChangedHtml(
        string jobTitle,
        ApplicationStatus newStatus,
        string? note)
    {
        var safeJobTitle = WebUtility.HtmlEncode(jobTitle);
        var statusVi = ToVietnameseApplicationStatus(newStatus);

        var noteBlock = string.IsNullOrWhiteSpace(note)
            ? string.Empty
            : $@"
                <div style=""margin-top:12px;padding:12px;border-radius:10px;background:#F9FAFB;border:1px solid #E5E7EB;"">
                  <div style=""font-size:12px;color:#6B7280;margin-bottom:6px;"">Ghi chú từ nhà tuyển dụng</div>
                  <div style=""font-size:14px;line-height:22px;color:#111827;"">{WebUtility.HtmlEncode(note)}</div>
                </div>";

        var body = $@"
            <h2 style=""margin:0 0 12px 0;font-size:20px;line-height:28px;color:#111827;"">Cập nhật hồ sơ ứng tuyển</h2>
            <p style=""margin:0 0 10px 0;color:#374151;font-size:14px;line-height:22px;"">
                Tin tuyển dụng: <strong>{safeJobTitle}</strong>
            </p>
            <p style=""margin:0 0 12px 0;color:#374151;font-size:14px;line-height:22px;"">
                Trạng thái mới: <strong>{WebUtility.HtmlEncode(statusVi)}</strong>
            </p>
            {noteBlock}
            <p style=""margin:16px 0 0 0;color:#6B7280;font-size:13px;line-height:20px;"">
                Vui lòng đăng nhập EnableVN để theo dõi tiến trình và trao đổi thêm nếu cần.
            </p>";

        return RenderLayoutHtml(
            title: "Cập nhật hồ sơ ứng tuyển",
            preheader: $"Trạng thái hồ sơ ứng tuyển của bạn đã được cập nhật: {statusVi}.",
            bodyHtml: body);
    }

    public static string RenderNotificationHtml(string title, string message)
    {
        var body = $@"
            <h2 style=""margin:0 0 12px 0;font-size:20px;line-height:28px;color:#111827;"">{WebUtility.HtmlEncode(title)}</h2>
            <p style=""margin:0;color:#374151;font-size:14px;line-height:22px;"">{WebUtility.HtmlEncode(message)}</p>";

        return RenderLayoutHtml(
            title: title,
            preheader: title,
            bodyHtml: body);
    }

    public static string ToVietnameseApplicationStatus(ApplicationStatus status)
        => status switch
        {
            ApplicationStatus.Pending => "Chờ xử lý",
            ApplicationStatus.Reviewing => "Đang xem xét",
            ApplicationStatus.Interview => "Mời phỏng vấn",
            ApplicationStatus.Accepted => "Đã được chấp nhận",
            ApplicationStatus.Rejected => "Không phù hợp / bị từ chối",
            ApplicationStatus.Withdrawn => "Đã rút hồ sơ",
            _ => status.ToString()
        };

    private static string RenderLayoutHtml(string title, string preheader, string bodyHtml)
    {
        // Preheader: hidden preview text in inbox. Keep short.
        var safeTitle = WebUtility.HtmlEncode(title);
        var safePreheader = WebUtility.HtmlEncode(preheader);

        // Basic responsive-ish layout (email clients are limited).
        // Use table container for compatibility.
        var sb = new StringBuilder();
        sb.AppendLine(@"<!doctype html>");
        sb.AppendLine(@"<html lang=""vi"">");
        sb.AppendLine(@"<head>");
        sb.AppendLine(@"  <meta charset=""utf-8"">");
        sb.AppendLine(@"  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">");
        sb.AppendLine($"  <title>{safeTitle}</title>");
        sb.AppendLine(@"</head>");
        sb.AppendLine(@"<body style=""margin:0;padding:0;background:#F3F4F6;"">");
        sb.AppendLine($@"  <div style=""display:none;max-height:0;overflow:hidden;opacity:0;color:transparent;"">{safePreheader}</div>");
        sb.AppendLine(@"  <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background:#F3F4F6;padding:24px 12px;"">");
        sb.AppendLine(@"    <tr><td align=""center"">");
        sb.AppendLine(@"      <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""width:100%;max-width:600px;background:#ffffff;border-radius:14px;overflow:hidden;border:1px solid #E5E7EB;"">");
        sb.AppendLine(@"        <tr>");
        sb.AppendLine(@"          <td style=""padding:16px 18px;background:#111827;color:#ffffff;font-family:Segoe UI,Roboto,Arial,sans-serif;"">");
        sb.AppendLine(@"            <div style=""font-size:16px;font-weight:700;letter-spacing:.3px;"">EnableVN</div>");
        sb.AppendLine(@"          </td>");
        sb.AppendLine(@"        </tr>");
        sb.AppendLine(@"        <tr>");
        sb.AppendLine(@"          <td style=""padding:18px;font-family:Segoe UI,Roboto,Arial,sans-serif;"">");
        sb.AppendLine(bodyHtml);
        sb.AppendLine(@"          </td>");
        sb.AppendLine(@"        </tr>");
        sb.AppendLine(@"        <tr>");
        sb.AppendLine(@"          <td style=""padding:14px 18px;background:#F9FAFB;border-top:1px solid #E5E7EB;font-family:Segoe UI,Roboto,Arial,sans-serif;color:#6B7280;font-size:12px;line-height:18px;"">");
        sb.AppendLine(@"            <div>Đây là email tự động từ EnableVN. Vui lòng không trả lời email này.</div>");
        sb.AppendLine(@"            <div style=""margin-top:6px;"">© EnableVN</div>");
        sb.AppendLine(@"          </td>");
        sb.AppendLine(@"        </tr>");
        sb.AppendLine(@"      </table>");
        sb.AppendLine(@"    </td></tr>");
        sb.AppendLine(@"  </table>");
        sb.AppendLine(@"</body>");
        sb.AppendLine(@"</html>");
        return sb.ToString();
    }
}

