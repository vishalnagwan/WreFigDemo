namespace WreFigDemo.Models.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? JobTitle { get; set; }
    public string? ResourceCategory { get; set; }   // matches FIG CallSheetRollup
    public string? WorkPhone { get; set; }
    public string? WorkMobilePhone { get; set; }
    public string DefaultShift { get; set; } = "AM";    // "AM" | "PM"
    public string? TruckAssignment { get; set; }
    public string? TruckId { get; set; }
    public string? ManagerName { get; set; }
    public bool IsActive { get; set; } = true;
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public ICollection<ScheduleEntry> ScheduleEntries { get; set; } = new List<ScheduleEntry>();
}
