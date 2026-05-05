using Microsoft.EntityFrameworkCore;
using WreFigDemo.Data;
using WreFigDemo.Models.Entities;
using WreFigDemo.Models.ViewModels;

namespace WreFigDemo.Services;

public class AuditService(AppDbContext db) : IAuditService
{
    public async Task LogAsync(string userId, string userName, string action,
                               string entityType, string entityId,
                               string? oldValue = null, string? newValue = null)
    {
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
        var prefix = $"{employeeId}:";
        var logs = await db.AuditLogs
            .AsNoTracking()
            .Where(a => a.EntityType == "ScheduleEntry" && a.EntityId.StartsWith(prefix))
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync();

        return logs.Select(l =>
        {
            var isSchedule = l.Action == "UpdateSchedule";
            var text = isSchedule
                ? $"Status changed {l.OldValue} → {l.NewValue}"
                : $"Note updated by {l.UserName}";

            var ago = (DateTime.UtcNow - l.Timestamp) switch
            {
                var t when t.TotalMinutes < 60  => $"{(int)t.TotalMinutes}m ago",
                var t when t.TotalHours  < 24   => $"{(int)t.TotalHours}h ago",
                var t when t.TotalDays   < 2    => "yesterday",
                var t                           => $"{(int)t.TotalDays} days ago"
            };

            return new HistoryItemVm
            {
                Text    = text + $" · by {l.UserName}",
                TimeAgo = ago,
                DotType = isSchedule ? "blue" : "gray"
            };
        }).ToList();
    }

    public async Task<List<AlertVm>> GetRecentAlertsAsync(int? branchId = null, int count = 50)
    {
        var query = db.AuditLogs.AsNoTracking()
            .OrderByDescending(a => a.Timestamp)
            .Take(count);

        var logs = await query.ToListAsync();

        return logs.Select(l => new AlertVm
        {
            Id         = l.Id,
            UserName   = l.UserName,
            Action     = l.Action,
            EntityType = l.EntityType,
            EntityId   = l.EntityId,
            OldValue   = l.OldValue,
            NewValue   = l.NewValue,
            Timestamp  = l.Timestamp
        }).ToList();
    }
}
