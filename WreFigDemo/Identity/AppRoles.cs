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

    // Can see all branches (Planner Dashboard mode)
    public static readonly string[] GlobalViewRoles =
        [PlannerDashboard, FieldSupervisor, DispatchSupervisor];

    // Scoped to their assigned branches only (Field Office-Dashboard mode)
    public static readonly string[] BranchScopedRoles =
        [Planner, Dispatcher, OtherEmployee];

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
