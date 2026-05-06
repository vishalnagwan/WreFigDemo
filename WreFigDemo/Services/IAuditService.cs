using WreFigDemo.Models.Entities;
using WreFigDemo.Models.ViewModels;

namespace WreFigDemo.Services;

public interface IAuditService
{
    Task LogAsync(string userId, string userName, string action,
                  string entityType, string entityId,
                  string? oldValue = null, string? newValue = null);

    Task<List<AlertVm>> GetRecentAlertsAsync(string? userId = null, int? branchId = null, int count = 50);
    Task<List<HistoryItemVm>> GetEmployeeHistoryAsync(int employeeId, DateOnly date, int count = 10);
}
