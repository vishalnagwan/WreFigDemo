using Microsoft.EntityFrameworkCore;
using WreFigDemo.Data;
using WreFigDemo.Identity;
using WreFigDemo.Models.Entities;
using WreFigDemo.Models.ViewModels;

namespace WreFigDemo.Services;

public class ScheduleService(IDbContextFactory<AppDbContext> dbFactory, IAuditService audit) : IScheduleService
{
    public async Task<ScheduleGridVm> GetGridAsync(int branchId, int year, int month)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var firstDay = new DateOnly(year, month, 1);
        var lastDay  = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

        var employees = await db.Employees
            .AsNoTracking()
            .Where(e => e.BranchId == branchId && e.IsActive)
            .OrderBy(e => e.DefaultShift)
            .ThenBy(e => e.Name)
            .ToListAsync();

        // Build a map of employee email → resource types from AppUser assignments
        var employeeEmails = employees
            .Where(e => !string.IsNullOrWhiteSpace(e.Email))
            .Select(e => e.Email!.ToLower())
            .Distinct()
            .ToList();

        var resourceTypesByEmail = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        if (employeeEmails.Count > 0)
        {
            var appUsers = await db.Set<AppUser>()
                .AsNoTracking()
                .Include(u => u.UserResourceTypes)
                .Where(u => u.Email != null && employeeEmails.Contains(u.Email.ToLower()))
                .ToListAsync();

            foreach (var u in appUsers)
            {
                if (u.Email is not null)
                    resourceTypesByEmail[u.Email] = u.UserResourceTypes
                        .Select(r => r.ResourceTypeName)
                        .OrderBy(r => r)
                        .ToList();
            }
        }

        var entries = await db.ScheduleEntries
            .AsNoTracking()
            .Where(e => e.Employee!.BranchId == branchId
                        && e.Date >= firstDay && e.Date <= lastDay)
            .ToListAsync();

        var entryMap = entries.ToDictionary(e => (e.EmployeeId, e.Date));

        var daysInMonth = DateTime.DaysInMonth(year, month);
        var days = Enumerable.Range(1, daysInMonth)
            .Select(d => new DayHeader
            {
                Day       = d,
                DayAbbr   = new DateTime(year, month, d).DayOfWeek.ToString()[..1],
                IsWeekend = new DateTime(year, month, d).DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday
            })
            .ToList();

        var rows = employees.Select(emp =>
        {
            var cells = new Dictionary<int, DayCell>();
            for (var d = 1; d <= daysInMonth; d++)
            {
                var date = new DateOnly(year, month, d);
                var isNew = !entryMap.TryGetValue((emp.Id, date), out var entry);
                cells[d] = new DayCell
                {
                    StatusCode = isNew ? DefaultCode(emp.DefaultShift, date) : entry!.StatusCode,
                    IsNew      = isNew,
                    HasNote    = !isNew && entry!.Note is not null
                };
            }
            var empResourceTypes = !string.IsNullOrWhiteSpace(emp.Email) &&
                                   resourceTypesByEmail.TryGetValue(emp.Email, out var rts)
                ? rts
                : [];

            return new EmployeeScheduleRow
            {
                EmployeeId       = emp.Id,
                EmployeeName     = emp.Name,
                DefaultShift     = emp.DefaultShift,
                TruckAssignment  = emp.TruckAssignment,
                TruckId          = emp.TruckId,
                ManagerName      = emp.ManagerName,
                WorkPhone        = emp.WorkPhone,
                WorkMobilePhone  = emp.WorkMobilePhone,
                ResourceTypes    = empResourceTypes,
                Cells            = cells
            };
        }).ToList();

        return new ScheduleGridVm
        {
            BranchId  = branchId,
            Year      = year,
            Month     = month,
            Days      = days,
            Employees = rows
        };
    }

    public async Task UpsertCellAsync(int employeeId, DateOnly date, string statusCode,
                                      string editorUserId, string editorUserName)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var existing = await db.ScheduleEntries
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.Date == date);

        var oldValue = existing?.StatusCode;

        if (existing is null)
        {
            db.ScheduleEntries.Add(new ScheduleEntry
            {
                EmployeeId = employeeId,
                Date       = date,
                StatusCode = statusCode,
                CreatedAt  = DateTime.UtcNow,
                CreatedBy  = editorUserId,
                UpdatedAt  = DateTime.UtcNow,
                UpdatedBy  = editorUserId
            });
        }
        else
        {
            existing.StatusCode = statusCode;
            existing.UpdatedAt  = DateTime.UtcNow;
            existing.UpdatedBy  = editorUserId;
        }

        await db.SaveChangesAsync();

        if (oldValue != statusCode)
            await audit.LogAsync(editorUserId, editorUserName, "UpdateSchedule",
                "ScheduleEntry", $"{employeeId}:{date:yyyy-MM-dd}", oldValue, statusCode);
    }

    public async Task BulkUpsertAsync(int branchId, int year, int month,
                                      Dictionary<int, Dictionary<int, string>> cells,
                                      string editorUserId, string editorUserName)
    {
        foreach (var (employeeId, dayCells) in cells)
        {
            foreach (var (day, code) in dayCells)
            {
                var date = new DateOnly(year, month, day);
                await UpsertCellAsync(employeeId, date, code, editorUserId, editorUserName);
            }
        }
    }

    public async Task<string?> GetNoteAsync(int employeeId, DateOnly date)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var entry = await db.ScheduleEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.Date == date);
        return entry?.Note;
    }

    public async Task UpsertNoteAsync(int employeeId, DateOnly date, string? note,
                                      string editorUserId, string editorUserName,
                                      string? currentStatusCode = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var existing = await db.ScheduleEntries
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.Date == date);

        if (existing is null)
        {
            db.ScheduleEntries.Add(new ScheduleEntry
            {
                EmployeeId = employeeId,
                Date       = date,
                StatusCode = currentStatusCode ?? "—",
                Note       = note,
                CreatedAt  = DateTime.UtcNow,
                CreatedBy  = editorUserId,
                UpdatedAt  = DateTime.UtcNow,
                UpdatedBy  = editorUserId
            });
        }
        else
        {
            existing.Note      = note;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = editorUserId;
        }

        await db.SaveChangesAsync();
        await audit.LogAsync(editorUserId, editorUserName, "UpdateNote",
            "ScheduleEntry", $"{employeeId}:{date:yyyy-MM-dd}", null, note);
    }

    private static string DefaultCode(string shift, DateOnly date)
    {
        var dow = new DateTime(date.Year, date.Month, date.Day).DayOfWeek;
        if (dow is DayOfWeek.Saturday or DayOfWeek.Sunday) return "—";
        return shift == "PM" ? "WP" : "WA";
    }
}
