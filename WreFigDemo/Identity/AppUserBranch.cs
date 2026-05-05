using WreFigDemo.Models.Entities;

namespace WreFigDemo.Identity;

public class AppUserBranch
{
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
}
