using Microsoft.EntityFrameworkCore;
using WreFigDemo.Data;
using WreFigDemo.Models.Entities;

namespace WreFigDemo.Services;

public class StatusCodeService(IDbContextFactory<AppDbContext> dbFactory) : IStatusCodeService
{
    private List<ScheduleStatusCode>? _cache;

    public async Task<List<ScheduleStatusCode>> GetAllAsync()
    {
        if (_cache is not null) return _cache;
        await using var db = await dbFactory.CreateDbContextAsync();
        _cache = await db.StatusCodes
            .AsNoTracking()
            .OrderBy(s => s.SortOrder)
            .ToListAsync();
        return _cache;
    }

    public string GetCssClass(string? code)
    {
        if (_cache is not null)
            return _cache.FirstOrDefault(s => s.Code == code)?.CssClass ?? "empty";

        return code?.ToLower() switch
        {
            "wa"  => "wa",
            "wp"  => "wp",
            "o"   => "o",
            "co"  => "co",
            "oc"  => "oc",
            "tr"  => "tr",
            "hd"  => "hd",
            "wx"  => "wx",
            // legacy fallback
            "pto" => "o",
            _     => "empty"
        };
    }

    public string GetLabel(string? code)
    {
        if (_cache is not null)
            return _cache.FirstOrDefault(s => s.Code == code)?.Label ?? code ?? "—";

        return code?.ToUpper() switch
        {
            "WA" or "WP" or "O" or "CO" or "OC" or "TR" or "HD" or "WX" => code.ToUpper(),
            "PTO" => "O",   // legacy fallback
            _     => "—"
        };
    }
}
