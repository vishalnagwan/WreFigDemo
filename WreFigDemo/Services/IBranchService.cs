using WreFigDemo.Models.ViewModels;

namespace WreFigDemo.Services;

public interface IBranchService
{
    Task<List<BranchSummaryVm>> GetBranchSummariesAsync(int year, int month, string? userId = null);
    Task<BranchSummaryVm?> GetBranchSummaryAsync(int branchId, int year, int month);
    Task<ComplianceVm> GetComplianceAsync(int year, int month, string? userId = null);
    Task<List<(int Id, string Name)>> GetBranchListAsync();
}
