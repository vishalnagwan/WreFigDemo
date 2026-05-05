using Microsoft.EntityFrameworkCore;
using WreFigDemo.Data;
using WreFigDemo.Models.Entities;

namespace WreFigDemo.Services;

public class StatusCodeService(AppDbContext db) : IStatusCodeService
{
    private List<ScheduleStatusCode>? _cache;

    public async Task<List<ScheduleStatusCode>> GetAllAsync()
    {
        _cache ??= await db.StatusCodes
            .AsNoTracking()
            .OrderBy(s => s.SortOrder)
            .ToListAsync();
        return _cache;
    }

    public string GetCssClass(string? code)
    {
        if (_cache is null) return "empty";
        return _cache.FirstOrDefault(s => s.Code == code)?.CssClass ?? "empty";
    }

    public string GetLabel(string? code)
    {
        if (_cache is null) return code ?? "—";
        return _cache.FirstOrDefault(s => s.Code == code)?.Label ?? code ?? "—";
    }
}
