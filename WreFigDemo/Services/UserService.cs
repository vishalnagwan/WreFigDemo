using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WreFigDemo.Data;
using WreFigDemo.Identity;
using WreFigDemo.Models.ViewModels;

namespace WreFigDemo.Services;

public class UserService(
    UserManager<AppUser> userManager,
    IDbContextFactory<AppDbContext> dbFactory,
    IAuditService audit) : IUserService
{
    public async Task<List<UserListVm>> GetUsersAsync()
    {
        var users = await userManager.Users
            .AsNoTracking()
            .Include(u => u.UserBranches)
            .ThenInclude(ub => ub.Branch)
            .ToListAsync();

        var result = new List<UserListVm>();

        foreach (var u in users)
        {
            var roles = await userManager.GetRolesAsync(u);
            result.Add(new UserListVm
            {
                Id          = u.Id,
                FullName    = u.FullName,
                Email       = u.Email ?? string.Empty,
                Role        = roles.FirstOrDefault() ?? string.Empty,
                IsActive    = u.IsActive,
                CreatedAt   = u.CreatedAt,
                BranchIds   = u.UserBranches.Select(ub => ub.BranchId).ToList(),
                BranchNames = u.UserBranches.Select(ub => ub.Branch?.Name ?? string.Empty).ToList()
            });
        }

        return result;
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> CreateUserAsync(
        CreateUserVm model, string actorId, string actorName)
    {
        var user = new AppUser
        {
            UserName  = model.Email,
            Email     = model.Email,
            FullName  = model.FullName,
            IsActive  = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, model.Password);
        if (!createResult.Succeeded)
            return (false, createResult.Errors.Select(e => e.Description));

        await userManager.AddToRoleAsync(user, model.Role);

        await using var db = await dbFactory.CreateDbContextAsync();
        foreach (var branchId in model.BranchIds)
            db.UserBranches.Add(new AppUserBranch { UserId = user.Id, BranchId = branchId });

        await db.SaveChangesAsync();

        await audit.LogAsync(actorId, actorName, "CreateUser", "AppUser", user.Id,
            null, $"{model.Email} / {model.Role}");

        return (true, []);
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> UpdateUserAsync(
        EditUserVm model, string actorId, string actorName)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var user = await userManager.Users
            .Include(u => u.UserBranches)
            .FirstOrDefaultAsync(u => u.Id == model.Id);

        if (user is null)
            return (false, ["User not found."]);

        var oldRole    = (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;
        var oldSummary = $"{user.Email} / {oldRole}";

        user.FullName = model.FullName;
        user.Email    = model.Email;
        user.UserName = model.Email;
        user.IsActive = model.IsActive;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return (false, updateResult.Errors.Select(e => e.Description));

        if (oldRole != model.Role)
        {
            if (!string.IsNullOrEmpty(oldRole))
                await userManager.RemoveFromRoleAsync(user, oldRole);
            await userManager.AddToRoleAsync(user, model.Role);
        }

        var existingLinks = await db.UserBranches.Where(ub => ub.UserId == user.Id).ToListAsync();
        db.UserBranches.RemoveRange(existingLinks);
        foreach (var branchId in model.BranchIds)
            db.UserBranches.Add(new AppUserBranch { UserId = user.Id, BranchId = branchId });

        await db.SaveChangesAsync();

        await audit.LogAsync(actorId, actorName, "UpdateUser", "AppUser", user.Id,
            oldSummary, $"{model.Email} / {model.Role}");

        return (true, []);
    }

    public async Task<(bool Success, string Error)> DeactivateUserAsync(
        string userId, string actorId, string actorName)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return (false, "User not found.");

        user.IsActive = false;
        await userManager.UpdateAsync(user);

        await audit.LogAsync(actorId, actorName, "DeactivateUser", "AppUser", userId,
            "Active", "Inactive");

        return (true, string.Empty);
    }
}
