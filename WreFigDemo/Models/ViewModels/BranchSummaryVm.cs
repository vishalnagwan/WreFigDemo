namespace WreFigDemo.Models.ViewModels;

public class BranchSummaryVm
{
    public int      BranchId      { get; set; }
    public string   BranchName    { get; set; } = string.Empty;
    public string   City          { get; set; } = string.Empty;
    public string   State         { get; set; } = string.Empty;
    public string   RegionName    { get; set; } = string.Empty;
    public bool     IsAcquisition { get; set; }
    public int      DriverCount   { get; set; }
    public double   FillRate      { get; set; }
    public DateTime? LastUpdated  { get; set; }
    public bool     HasNotes      { get; set; }

    public string Status => IsAcquisition ? "acquisition"
        : FillRate >= 85 ? "green"
        : FillRate >= 75 ? "amber"
        : "red";

    public string LastUpdatedDisplay => LastUpdated.HasValue
        ? LastUpdated.Value > DateTime.UtcNow.AddHours(-1)
            ? $"{(int)(DateTime.UtcNow - LastUpdated.Value).TotalMinutes} min ago"
            : LastUpdated.Value.ToString("ddd h:mm tt")
        : "Never";
}
