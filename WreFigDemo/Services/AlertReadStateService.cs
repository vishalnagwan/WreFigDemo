namespace WreFigDemo.Services;

/// <summary>
/// Scoped per-circuit service that tracks which alert IDs the user has already seen,
/// so the "Unread" filter reflects genuinely unseen alerts.
/// </summary>
public class AlertReadStateService
{
    private readonly HashSet<int> _seenIds = [];

    public bool IsUnread(int id) => !_seenIds.Contains(id);

    /// <summary>Mark all supplied IDs as read (called after the user views the alerts page).</summary>
    public void MarkAllRead(IEnumerable<int> ids) => _seenIds.UnionWith(ids);
}
