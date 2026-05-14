using WreFigDemo.Models.Entities;

namespace WreFigDemo.Models;

/// <summary>
/// Canonical, ordered list of instruction sections matching the FIG spreadsheet layout.
/// Sections are static — only their content (BranchInstruction rows) is branch-specific.
/// </summary>
public static class InstructionSections
{
    public sealed record SectionDef(string Key, string Title, InstructionPosition Position);

    public static readonly SectionDef[] All =
    [
        new("ServiceLimitations",     "Service Limitations",                                                                   InstructionPosition.Top),
        new("LobSpecificInfo",        "LOB Specific Info",                                                                    InstructionPosition.Top),
        new("OnCallHours",            "On Call Hours",                                                                        InstructionPosition.Top),
        new("TruckEquipmentAlerts",   "Truck / Equipment Status Alerts",                                                      InstructionPosition.Top),
        new("AssignmentRouting",      "Assignment / Routing Notes",                                                           InstructionPosition.Top),
        new("SupervisorEscalation",   "Supervisor Contact / Escalation Notes",                                                InstructionPosition.Top),
        new("Disposal",               "Disposal Considerations",                                                              InstructionPosition.Top),
        new("TruckBreakdowns",        "Truck Breakdowns / Accidents / Safety / Environmental (Spills, etc.)",                 InstructionPosition.Top),
        new("OtherNotes",             "Other Notes",                                                                          InstructionPosition.Top),
        new("DispatchConsiderations", "Dispatch Considerations",                                                              InstructionPosition.Bottom),
        new("LogisticsConsiderations","Logistics Considerations",                                                             InstructionPosition.Bottom),
    ];

    public static SectionDef? Find(string key) =>
        All.FirstOrDefault(s => s.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
}
