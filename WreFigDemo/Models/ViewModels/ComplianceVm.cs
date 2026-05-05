namespace WreFigDemo.Models.ViewModels;

public class ComplianceVm
{
    public int Year { get; set; }
    public int Month { get; set; }
    public List<BranchComplianceRow> Branches { get; set; } = [];

    public double AverageFillRate =>
        Branches.Count == 0 ? 0 : Branches.Average(b => b.FillRate);

    public int UpToDateCount   => Branches.Count(b => b.FillRate >= 85);
    public int BelowThreshold  => Branches.Count(b => b.FillRate < 75);
}

public class BranchComplianceRow
{
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public double FillRate { get; set; }
    public int DaysComplete { get; set; }
    public int TotalWorkdays { get; set; }
    public DateTime? LastUpdated { get; set; }

    public string Status => FillRate >= 85 ? "green" : FillRate >= 60 ? "amber" : "red";

    public string LastUpdatedDisplay => LastUpdated.HasValue
        ? LastUpdated.Value.ToString("ddd h:mm tt")
        : "Never";
}
