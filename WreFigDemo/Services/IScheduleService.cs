using WreFigDemo.Models.ViewModels;

namespace WreFigDemo.Services;

public interface IScheduleService
{
    Task<ScheduleGridVm> GetGridAsync(int branchId, int year, int month);
    Task UpsertCellAsync(int employeeId, DateOnly date, string statusCode, string editorUserId, string editorUserName);
    Task BulkUpsertAsync(int branchId, int year, int month, Dictionary<int, Dictionary<int, string>> cells, string editorUserId, string editorUserName);
    Task<string?> GetNoteAsync(int employeeId, DateOnly date);
    Task UpsertNoteAsync(int employeeId, DateOnly date, string? note, string editorUserId, string editorUserName);
}
