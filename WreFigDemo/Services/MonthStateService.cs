using Microsoft.EntityFrameworkCore;
using WreFigDemo.Data;
using WreFigDemo.Models.Entities;

namespace WreFigDemo.Services;

public class MonthStateService(IDbContextFactory<AppDbContext> factory) : IMonthStateService
{
    public async Task<List<MonthLock>> GetAllAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.MonthLocks
            .OrderByDescending(m => m.Year)
            .ThenByDescending(m => m.Month)
            .ToListAsync();
    }

    public async Task EnsureDefaultsAsync(string modifiedBy)
    {
        await using var db = await factory.CreateDbContextAsync();
        var now = DateTime.UtcNow;

        // Seed current month + next 2 months as open if not already tracked.
        for (int offset = -1; offset <= 2; offset++)
        {
            var target = new DateTime(now.Year, now.Month, 1).AddMonths(offset);
            bool exists = await db.MonthLocks
                .AnyAsync(m => m.Year == target.Year && m.Month == target.Month);

            if (!exists)
            {
                db.MonthLocks.Add(new MonthLock
                {
                    Year       = target.Year,
                    Month      = target.Month,
                    IsOpen     = true,
                    ModifiedAt = now,
                    ModifiedBy = modifiedBy,
                });
            }
        }

        await db.SaveChangesAsync();
    }

    public async Task OpenMonthAsync(int year, int month, string modifiedBy)
        => await SetStateAsync(year, month, true, modifiedBy);

    public async Task CloseMonthAsync(int year, int month, string modifiedBy)
        => await SetStateAsync(year, month, false, modifiedBy);

    private async Task SetStateAsync(int year, int month, bool isOpen, string modifiedBy)
    {
        await using var db = await factory.CreateDbContextAsync();
        var record = await db.MonthLocks
            .FirstOrDefaultAsync(m => m.Year == year && m.Month == month);

        if (record is null)
        {
            record = new MonthLock { Year = year, Month = month };
            db.MonthLocks.Add(record);
        }

        record.IsOpen     = isOpen;
        record.ModifiedAt = DateTime.UtcNow;
        record.ModifiedBy = modifiedBy;
        await db.SaveChangesAsync();
    }
}
