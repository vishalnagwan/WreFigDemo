using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WreFigDemo.Data;
using WreFigDemo.Identity;
using WreFigDemo.Models.ViewModels;

namespace WreFigDemo.Services;

public class BranchService(AppDbContext db, UserManager<AppUser> userManager) : IBranchService
{
    public async Task<List<BranchSummaryVm>> GetBranchSummariesAsync(int year, int month, string? userId = null)
    {
        var branchIds = await GetAccessibleBranchIdsAsync(userId);

        var branches = await db.Branches
            .AsNoTracking()
            .Where(b => b.IsActive && (branchIds == null || branchIds.Contains(b.Id)))
            .Include(b => b.Region)
            .OrderBy(b => b.Region.Name)
            .ThenBy(b => b.Name)
            .ToListAsync();

        var workdays = GetWorkdayCount(year, month);
        var firstDay = new DateOnly(year, month, 1);
        var lastDay  = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

        var entries = await db.ScheduleEntries
            .AsNoTracking()
            .Where(e => e.Date >= firstDay && e.Date <= lastDay
                        && (branchIds == null || branchIds.Contains(e.Employee!.BranchId)))
            .GroupBy(e => e.Employee!.BranchId)
            .Select(g => new
            {
                BranchId    = g.Key,
                FilledCount = g.Count(e => e.StatusCode != "—" && e.StatusCode != null),
                LastUpdated = g.Max(e => (DateTime?)e.UpdatedAt)
            })
            .ToListAsync();

        var entriesMap = entries.ToDictionary(e => e.BranchId);

        var employeeCountMap = await db.Employees
            .AsNoTracking()
            .Where(e => e.IsActive)
            .GroupBy(e => e.BranchId)
            .Select(g => new { BranchId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.BranchId, g => g.Count);

        return branches.Select(b =>
        {
            var empCount    = employeeCountMap.GetValueOrDefault(b.Id, 0);
            var totalSlots  = empCount * workdays;
            var filled      = entriesMap.TryGetValue(b.Id, out var e) ? e.FilledCount : 0;
            var fillRate    = totalSlots == 0 ? 0.0 : Math.Round(filled * 100.0 / totalSlots, 1);

            return new BranchSummaryVm
            {
                BranchId      = b.Id,
                BranchName    = b.Name,
                City          = b.City,
                State         = b.State,
                RegionName    = b.Region?.Name ?? string.Empty,
                IsAcquisition = b.IsAcquisition,
                DriverCount   = empCount,
                FillRate      = fillRate,
                LastUpdated   = entriesMap.TryGetValue(b.Id, out var ev) ? ev.LastUpdated : null
            };
        }).ToList();
    }

    public async Task<BranchSummaryVm?> GetBranchSummaryAsync(int branchId, int year, int month)
    {
        var summaries = await GetBranchSummariesAsync(year, month);
        return summaries.FirstOrDefault(s => s.BranchId == branchId);
    }

    public async Task<ComplianceVm> GetComplianceAsync(int year, int month)
    {
        var summaries = await GetBranchSummariesAsync(year, month);
        var workdays  = GetWorkdayCount(year, month);

        var empCounts = await db.Employees
            .AsNoTracking()
            .Where(e => e.IsActive)
            .GroupBy(e => e.BranchId)
            .Select(g => new { BranchId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.BranchId, g => g.Count);

        var rows = summaries.Select(s => new BranchComplianceRow
        {
            BranchId      = s.BranchId,
            BranchName    = s.BranchName,
            FillRate      = s.FillRate,
            DaysComplete  = (int)Math.Round(s.FillRate * workdays / 100),
            TotalWorkdays = workdays,
            LastUpdated   = s.LastUpdated
        }).ToList();

        return new ComplianceVm { Year = year, Month = month, Branches = rows };
    }

    private async Task<HashSet<int>?> GetAccessibleBranchIdsAsync(string? userId)
    {
        if (userId == null) return null;

        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var roles = await userManager.GetRolesAsync(user);

        if (AppRoles.GlobalViewRoles.Any(r => roles.Contains(r)))
            return null; // all branches

        return (await db.UserBranches
            .AsNoTracking()
            .Where(ub => ub.UserId == userId)
            .Select(ub => ub.BranchId)
            .ToListAsync()).ToHashSet();
    }

    private static int GetWorkdayCount(int year, int month)
    {
        var days  = DateTime.DaysInMonth(year, month);
        var count = 0;
        for (var d = 1; d <= days; d++)
        {
            var dow = new DateTime(year, month, d).DayOfWeek;
            if (dow != DayOfWeek.Saturday && dow != DayOfWeek.Sunday)
                count++;
        }
        return count;
    }
}
