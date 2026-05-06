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

    // Can write (paint) schedule status codes — writes to FIG per the access flow
    public static readonly string[] WriteRoles =
        [PlannerDashboard, FieldSupervisor, DispatchSupervisor];

    // Can add / edit supervisor notes — write roles + Planner (read & chase)
    public static readonly string[] NoteRoles =
        [PlannerDashboard, FieldSupervisor, DispatchSupervisor, Planner];

    // Can view compliance report and change alerts
    public static readonly string[] AlertComplianceRoles =
        [PlannerDashboard, FieldSupervisor, DispatchSupervisor, Planner];

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
