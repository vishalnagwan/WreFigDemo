using Microsoft.AspNetCore.Identity;

namespace WreFigDemo.Identity;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<AppUserBranch>       UserBranches       { get; set; } = new List<AppUserBranch>();
    public ICollection<AppUserResourceType> UserResourceTypes  { get; set; } = new List<AppUserResourceType>();
}
