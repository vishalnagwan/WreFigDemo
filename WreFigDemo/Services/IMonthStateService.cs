using WreFigDemo.Models.Entities;

namespace WreFigDemo.Services;

public interface IMonthStateService
{
    /// <summary>Returns all month locks ordered by year/month descending.</summary>
    Task<List<MonthLock>> GetAllAsync();

    /// <summary>
    /// Ensures records exist for the current month and the next two months,
    /// seeding them as open if missing. Called on first load of the Rollover Console.
    /// </summary>
    Task EnsureDefaultsAsync(string modifiedBy);

    /// <summary>Opens a previously closed month.</summary>
    Task OpenMonthAsync(int year, int month, string modifiedBy);

    /// <summary>Closes (hides) a month — prior-month end-of-cycle action.</summary>
    Task CloseMonthAsync(int year, int month, string modifiedBy);
}
