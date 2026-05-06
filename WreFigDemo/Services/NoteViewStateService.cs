namespace WreFigDemo.Services;

/// <summary>
/// Scoped per-circuit service that remembers which noted cells and branches
/// the user has already opened, so the "unviewed note" highlight clears on view.
/// </summary>
public class NoteViewStateService
{
    private readonly HashSet<string> _viewedCells    = [];
    private readonly HashSet<int>    _viewedBranches = [];

    // Key format: "{employeeId}:{date:yyyy-MM-dd}"
    public void MarkCellViewed(int employeeId, DateOnly date)
        => _viewedCells.Add(CellKey(employeeId, date));

    public void UnmarkCellViewed(int employeeId, DateOnly date)
        => _viewedCells.Remove(CellKey(employeeId, date));

    public bool IsCellViewed(int employeeId, DateOnly date)
        => _viewedCells.Contains(CellKey(employeeId, date));

    public void MarkBranchViewed(int branchId)
        => _viewedBranches.Add(branchId);

    public void UnmarkBranchViewed(int branchId)
        => _viewedBranches.Remove(branchId);

    public bool IsBranchViewed(int branchId)
        => _viewedBranches.Contains(branchId);

    private static string CellKey(int employeeId, DateOnly date)
        => $"{employeeId}:{date:yyyy-MM-dd}";
}
