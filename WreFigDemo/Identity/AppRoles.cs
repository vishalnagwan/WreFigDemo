namespace WreFigDemo.Identity;

public static class AppRoles
{
    public const string PlannerDashboard   = "PlannerDashboard";    // Central Dispatch equivalent
    public const string FieldSupervisor    = "FieldSupervisor";
    public const string DispatchSupervisor = "DispatchSupervisor";
    public const string Planner            = "Planner";
    public const string Dispatcher         = "Dispatcher";
    public const string OtherEmployee      = "OtherEmployee";

    public static readonly string[] All =
    [
        PlannerDashboard, FieldSupervisor, DispatchSupervisor,
        Planner, Dispatcher, OtherEmployee
    ];

    // Can see ALL branches — Central Dispatch / admin only
    public static readonly string[] GlobalViewRoles =
        [PlannerDashboard];

    // Scoped to their assigned branches only
    public static readonly string[] BranchScopedRoles =
        [FieldSupervisor, DispatchSupervisor, Planner, Dispatcher, OtherEmployee];

    // Can write (paint) schedule status codes — PPT: Planner has cross-branch R/W
    public static readonly string[] WriteRoles =
        [PlannerDashboard, FieldSupervisor, DispatchSupervisor, Planner];

    // Can add / edit supervisor notes — write roles + Dispatcher (note on real-time changes)
    public static readonly string[] NoteRoles =
        [PlannerDashboard, FieldSupervisor, DispatchSupervisor, Planner];

    // Can view compliance (fill-rate) report — excludes Dispatcher and OtherEmployee
    public static readonly string[] ComplianceRoles =
        [PlannerDashboard, FieldSupervisor, DispatchSupervisor, Planner];

    // Can view change-alerts inbox — PPT: Dispatcher sees "24-hour alerts feed front-and-center"
    public static readonly string[] AlertRoles =
        [PlannerDashboard, FieldSupervisor, DispatchSupervisor, Planner, Dispatcher];

    // Legacy alias — kept so any existing references still compile
    public static readonly string[] AlertComplianceRoles =
        [PlannerDashboard, FieldSupervisor, DispatchSupervisor, Planner];

    // Can access the month rollover console — PPT: Dispatch Supervisor owns this ritual; Admin can too
    public static readonly string[] RolloverRoles =
        [PlannerDashboard, DispatchSupervisor];

    public static bool IsGlobalRole(string role) =>
        GlobalViewRoles.Contains(role, StringComparer.OrdinalIgnoreCase);

    public static string DisplayName(string role) => role switch
    {
        PlannerDashboard   => "Planner Dashboard",
        FieldSupervisor    => "Field Supervisor",
        DispatchSupervisor => "Dispatch Supervisor",
        Planner            => "Planner",
        Dispatcher         => "Dispatcher",
        OtherEmployee      => "Other Employee",
        _                  => role
    };
}
