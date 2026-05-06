using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WreFigDemo.Data;
using WreFigDemo.Identity;
using WreFigDemo.Models.Entities;
using WreFigDemo.Models.ViewModels;

namespace WreFigDemo.Services;

public class AuditService(IDbContextFactory<AppDbContext> dbFactory,
                           UserManager<AppUser> userManager) : IAuditService
{
    public async Task LogAsync(string userId, string userName, string action,
                               string entityType, string entityId,
                               string? oldValue = null, string? newValue = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        db.AuditLogs.Add(new AuditLog
        {
            UserId     = userId,
            UserName   = userName,
            Action     = action,
            EntityType = entityType,
            EntityId   = entityId,
            OldValue   = oldValue,
            NewValue   = newValue,
            Timestamp  = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    public async Task<List<HistoryItemVm>> GetEmployeeHistoryAsync(int employeeId, DateOnly date, int count = 10)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        // Exact day match — EntityId format is "{employeeId}:{yyyy-MM-dd}"
        var exactId = $"{employeeId}:{date:yyyy-MM-dd}";
        var logs = await db.AuditLogs
            .AsNoTracking()
            .Where(a => a.EntityType == "ScheduleEntry" && a.EntityId == exactId)
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync();

        return logs.Select(l =>
        {
            var isSchedule = l.Action == "UpdateSchedule";

            // "Note updated by X · by X" was a duplicate — keep a single "by"
            var text = isSchedule
                ? $"Status changed {l.OldValue} → {l.NewValue}"
                : "Note updated";

            var ago = (DateTime.UtcNow - l.Timestamp) switch
            {
                var t when t.TotalMinutes < 2   => "just now",
                var t when t.TotalMinutes < 60  => $"{(int)t.TotalMinutes}m ago",
                var t when t.TotalHours   < 24  => $"{(int)t.TotalHours}h ago",
                var t when t.TotalDays    < 2   => "yesterday",
                var t                           => $"{(int)t.TotalDays}d ago"
            };

            return new HistoryItemVm
            {
                Text    = $"{text} · by {l.UserName}",
                TimeAgo = ago,
                DotType = isSchedule ? "blue" : "gray"
            };
        }).ToList();
    }

    public async Task<List<AlertVm>> GetRecentAlertsAsync(string? userId = null, int? branchId = null, int count = 50)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        // Resolve which branch IDs this caller may see
        HashSet<int>? allowedBranchIds = null;

        if (branchId.HasValue)
        {
            // BranchGrid: scope to a single specific branch
            allowedBranchIds = [branchId.Value];
        }
        else if (userId != null)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var roles = await userManager.GetRolesAsync(user);
                var isGlobal = AppRoles.GlobalViewRoles.Any(r => roles.Contains(r));
                if (!isGlobal)
                {
                    allowedBranchIds = (await db.UserBranches
                        .AsNoTracking()
                        .Where(ub => ub.UserId == userId)
                        .Select(ub => ub.BranchId)
                        .ToListAsync()).ToHashSet();
                }
            }
        }

        // Fetch recent ScheduleEntry logs only; non-branch entities (AppUser, etc.)
        // are only visible to global admins (allowedBranchIds == null)
        IQueryable<AuditLog> query = db.AuditLogs.AsNoTracking()
            .OrderByDescending(a => a.Timestamp);

        if (allowedBranchIds != null)
        {
            // Get employee IDs that belong to the allowed branches
            var employeeIds = await db.Employees
                .AsNoTracking()
                .Where(e => allowedBranchIds.Contains(e.BranchId))
                .Select(e => e.Id.ToString())
                .ToListAsync();

            // EntityId format for ScheduleEntry: "{employeeId}:{date}"
            // Filter to ScheduleEntry logs whose leading segment matches an allowed employee
            var logs = await query
                .Where(a => a.EntityType == "ScheduleEntry")
                .Take(count * 4)   // over-fetch; we'll filter in memory
                .ToListAsync();

            var result = logs
                .Where(l =>
                {
                    var colon = l.EntityId.IndexOf(':');
                    var empIdStr = colon > 0 ? l.EntityId[..colon] : l.EntityId;
                    return employeeIds.Contains(empIdStr);
                })
                .Take(count)
                .ToList();

            return result.Select(ToVm).ToList();
        }
        else
        {
            // Global admin — return all recent logs
            var logs = await query.Take(count).ToListAsync();
            return logs.Select(ToVm).ToList();
        }
    }

    private static AlertVm ToVm(AuditLog l) => new()
    {
        Id         = l.Id,
        UserName   = l.UserName,
        Action     = l.Action,
        EntityType = l.EntityType,
        EntityId   = l.EntityId,
        OldValue   = l.OldValue,
        NewValue   = l.NewValue,
        Timestamp  = l.Timestamp
    };
}
