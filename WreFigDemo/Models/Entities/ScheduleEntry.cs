namespace WreFigDemo.Models.Entities;

public class ScheduleEntry
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public DateOnly Date { get; set; }

    /// <summary>WA | WP | PTO | CO | OC | TR | — </summary>
    public string StatusCode { get; set; } = "—";

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
