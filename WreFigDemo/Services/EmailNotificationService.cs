using Azure;
using Azure.Communication.Email;

namespace WreFigDemo.Services;

public class EmailNotificationService(IConfiguration config, ILogger<EmailNotificationService> logger)
    : IEmailNotificationService
{
    private readonly string _connectionString = config["AzureCommunicationServices:ConnectionString"] ?? string.Empty;
    private readonly string _fromAddress      = config["AzureCommunicationServices:FromAddress"] ?? "noreply@wre-fig.com";

    public async Task SendComplianceAlertAsync(string toEmail, string toName, string branchName,
                                               double fillRate, int year, int month)
    {
        var monthName = new DateTime(year, month, 1).ToString("MMMM yyyy");
        var subject   = $"WRE FIG – Low Compliance Alert: {branchName}";
        var body      = $"""
            <p>Hi {toName},</p>
            <p>Branch <strong>{branchName}</strong> has a fill rate of <strong>{fillRate:0.0}%</strong>
            for <strong>{monthName}</strong>, which is below the 75% threshold.</p>
            <p>Please log in to the WRE FIG system to review and update the schedule.</p>
            <p>— WRE FIG Automated Notification</p>
            """;

        await SendAsync(toEmail, toName, subject, body);
    }

    public async Task SendScheduleChangeNotificationAsync(string toEmail, string toName,
                                                          string employeeName, string date,
                                                          string oldCode, string newCode)
    {
        var subject = $"WRE FIG – Schedule Change: {employeeName} on {date}";
        var body    = $"""
            <p>Hi {toName},</p>
            <p>A schedule change was recorded for <strong>{employeeName}</strong> on <strong>{date}</strong>:
            <strong>{oldCode ?? "—"}</strong> → <strong>{newCode}</strong>.</p>
            <p>— WRE FIG Automated Notification</p>
            """;

        await SendAsync(toEmail, toName, subject, body);
    }

    private async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            logger.LogWarning("ACS connection string not configured; skipping email to {Email}", toEmail);
            return;
        }

        try
        {
            var client  = new EmailClient(_connectionString);
            var message = new EmailMessage(
                senderAddress: _fromAddress,
                content: new EmailContent(subject) { Html = htmlBody },
                recipients: new EmailRecipients([new EmailAddress(toEmail, toName)]));

            await client.SendAsync(WaitUntil.Completed, message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email}", toEmail);
        }
    }
}
