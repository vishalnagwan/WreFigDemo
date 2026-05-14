using WreFigDemo.Models.ViewModels;

namespace WreFigDemo.Services;

public interface IBranchInstructionService
{
    Task<BranchInstructionsVm> GetForBranchAsync(int branchId);
    Task SaveAllAsync(BranchInstructionsVm vm, string userId, string userName);
}
