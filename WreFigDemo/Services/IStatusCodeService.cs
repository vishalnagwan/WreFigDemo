using WreFigDemo.Models.Entities;

namespace WreFigDemo.Services;

public interface IStatusCodeService
{
    Task<List<ScheduleStatusCode>> GetAllAsync();
    string GetCssClass(string? code);
    string GetLabel(string? code);
}
