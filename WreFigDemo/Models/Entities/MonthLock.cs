namespace WreFigDemo.Models.Entities;

/// <summary>
/// Tracks which year/month combinations are "open" (visible and editable) vs "closed"
/// (hidden from the planning grid). Managed exclusively by Dispatch Supervisor and Admin
/// via the Rollover Console.
/// </summary>
public class MonthLock
{
    public int Id { get; set; }

    public int Year  { get; set; }
    public int Month { get; set; }

    /// <summary>False = closed/hidden; True = open (default for current and next month).</summary>
    public bool IsOpen { get; set; } = true;

    public DateTime ModifiedAt  { get; set; } = DateTime.UtcNow;
    public string   ModifiedBy  { get; set; } = string.Empty;
}
