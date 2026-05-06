using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WreFigDemo.Data;
using WreFigDemo.Identity;
using WreFigDemo.Models.ViewModels;

namespace WreFigDemo.Services;

public class BranchService(IDbContextFactory<AppDbContext> dbFactory, UserManager<AppUser> userManager) : IBranchService
{
    public async Task<List<BranchSummaryVm>> GetBranchSummariesAsync(int year, int month, string? userId = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

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

        var workdayDates = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
            .Select(d => new DateOnly(year, month, d))
            .Where(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
            .ToHashSet();

        var rawEntries = await db.ScheduleEntries
            .AsNoTracking()
            .Where(e => e.Date >= firstDay && e.Date <= lastDay
                        && (branchIds == null || branchIds.Contains(e.Employee!.BranchId)))
            .Select(e => new { BranchId = e.Employee!.BranchId, e.Date, e.StatusCode, e.UpdatedAt })
            .ToListAsync();

        var entriesMap = rawEntries
            .GroupBy(e => e.BranchId)
            .ToDictionary(g => g.Key, g => new
            {
                UnfilledCount = g.Count(e => workdayDates.Contains(e.Date)
                                             && (e.StatusCode == "—" || e.StatusCode == null)),
                LastUpdated   = g.Max(e => (DateTime?)e.UpdatedAt)
            });

        var employeeCountMap = await db.Employees
            .AsNoTracking()
            .Where(e => e.IsActive)
            .GroupBy(e => e.BranchId)
            .Select(g => new { BranchId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.BranchId, g => g.Count);

        // Branches that have at least one supervisor note this month
        var branchesWithNotes = (await db.ScheduleEntries
            .AsNoTracking()
            .Where(e => e.Date >= firstDay && e.Date <= lastDay
                        && e.Note != null && e.Note != ""
                        && (branchIds == null || branchIds.Contains(e.Employee!.BranchId)))
            .Select(e => e.Employee!.BranchId)
            .Distinct()
            .ToListAsync()).ToHashSet();

        return branches.Select(b =>
        {
            var empCount    = employeeCountMap.GetValueOrDefault(b.Id, 0);
            var totalSlots  = empCount * workdays;
            var unfilled    = entriesMap.TryGetValue(b.Id, out var e) ? e.UnfilledCount : 0;
            var fillRate    = totalSlots == 0 ? 0.0 : Math.Round((totalSlots - unfilled) * 100.0 / totalSlots, 1);

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
                LastUpdated   = entriesMap.TryGetValue(b.Id, out var ev) ? ev.LastUpdated : null,
                HasNotes      = branchesWithNotes.Contains(b.Id)
            };
        }).ToList();
    }

    public async Task<BranchSummaryVm?> GetBranchSummaryAsync(int branchId, int year, int month)
    {
        var summaries = await GetBranchSummariesAsync(year, month);
        return summaries.FirstOrDefault(s => s.BranchId == branchId);
    }

    public async Task<List<(int Id, string Name)>> GetBranchListAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var rows = await db.Branches
            .AsNoTracking()
            .Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .Select(b => new { b.Id, b.Name })
            .ToListAsync();
        return rows.Select(b => (b.Id, b.Name)).ToList();
    }

    public async Task<ComplianceVm> GetComplianceAsync(int year, int month, string? userId = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var summaries = await GetBranchSummariesAsync(year, month, userId);
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
            return null;

        await using var db = await dbFactory.CreateDbContextAsync();
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
