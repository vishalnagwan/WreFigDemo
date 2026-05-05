namespace WreFigDemo.Models.ViewModels;

public class ScheduleGridVm
{
    public int  BranchId { get; set; }
    public int  Year     { get; set; }
    public int  Month    { get; set; }
    public List<DayHeader>          Days      { get; set; } = [];
    public List<EmployeeScheduleRow> Employees { get; set; } = [];
}

public class DayHeader
{
    public int    Day       { get; set; }
    public string DayAbbr   { get; set; } = string.Empty;
    public bool   IsWeekend { get; set; }
}

public class EmployeeScheduleRow
{
    public int    EmployeeId      { get; set; }
    public string EmployeeName    { get; set; } = string.Empty;
    public string DefaultShift    { get; set; } = "AM";
    public string? TruckAssignment { get; set; }
    public string? TruckId         { get; set; }
    public string? ManagerName     { get; set; }
    /// <summary>Key = day of month (1..31)</summary>
    public Dictionary<int, DayCell> Cells { get; set; } = [];
}

public class HistoryItemVm
{
    public string Text      { get; set; } = string.Empty;
    public string TimeAgo   { get; set; } = string.Empty;
    public string DotType   { get; set; } = "gray"; // "blue" | "gray"
}

public class DayCell
{
    public string  StatusCode { get; set; } = "—";
    public bool    IsNew      { get; set; }
    public bool    HasNote    { get; set; }
}
