namespace WreFigDemo.Services;

public interface IEmailNotificationService
{
    Task SendComplianceAlertAsync(string toEmail, string toName, string branchName,
                                  double fillRate, int year, int month);
    Task SendScheduleChangeNotificationAsync(string toEmail, string toName,
                                             string employeeName, string date, string oldCode, string newCode);
}
