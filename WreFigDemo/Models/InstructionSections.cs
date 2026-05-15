using WreFigDemo.Models.Entities;

namespace WreFigDemo.Models;

/// <summary>
/// Canonical, ordered list of instruction sections matching the FIG spreadsheet layout.
/// Sections are static — only their content (BranchInstruction rows) is branch-specific.
/// </summary>
public static class InstructionSections
{
    public sealed record SectionDef(string Key, string Title, string ShortTitle, InstructionPosition Position);

    public static readonly SectionDef[] All =
    [
        new("ServiceLimitations",     "Service Limitations",                                                              "LIMITS",     InstructionPosition.Top),
        new("LobSpecificInfo",        "LOB Specific Info",                                                               "LOB INFO",   InstructionPosition.Top),
        new("OnCallHours",            "On Call Hours",                                                                   "ON CALL",    InstructionPosition.Top),
        new("TruckEquipmentAlerts",   "Truck / Equipment Status Alerts",                                                 "TRUCKS",     InstructionPosition.Top),
        new("AssignmentRouting",      "Assignment / Routing Notes",                                                      "ROUTING",    InstructionPosition.Top),
        new("SupervisorEscalation",   "Supervisor Contact / Escalation Notes",                                           "ESCALATE",   InstructionPosition.Top),
        new("Disposal",               "Disposal Considerations",                                                         "DISPOSAL",   InstructionPosition.Top),
        new("TruckBreakdowns",        "Truck Breakdowns / Accidents / Safety / Environmental (Spills, etc.)",            "BREAKDOWN",  InstructionPosition.Top),
        new("OtherNotes",             "Other Notes",                                                                     "OTHER",      InstructionPosition.Top),
        new("DispatchConsiderations", "Dispatch Considerations",                                                         "DISPATCH",   InstructionPosition.Bottom),
        new("LogisticsConsiderations","Logistics Considerations",                                                        "LOGISTICS",  InstructionPosition.Bottom),
    ];

    public static SectionDef? Find(string key) =>
        All.FirstOrDefault(s => s.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
}
