namespace WreFigDemo.Models.ViewModels;

public class AlertVm
{
    public int      Id         { get; set; }
    public string   UserName   { get; set; } = string.Empty;
    public string   Action     { get; set; } = string.Empty;
    public string   EntityType { get; set; } = string.Empty;
    public string   EntityId   { get; set; } = string.Empty;
    public string?  OldValue   { get; set; }
    public string?  NewValue   { get; set; }
    public DateTime Timestamp  { get; set; }

    public bool IsUnread { get; set; } = true;

    /// <summary>
    /// Urgency is based on how close the scheduled date (parsed from EntityId) is to today.
    /// EntityId format: "{employeeId}:{yyyy-MM-dd}"
    /// Past dates (shift already happened) fall back to audit-log recency.
    /// </summary>
    public string Urgency
    {
        get
        {
            var parts = EntityId.Split(':');
            if (parts.Length == 2 && DateOnly.TryParse(parts[1], out var schedDate))
            {
                var today     = DateOnly.FromDateTime(DateTime.UtcNow);
                var daysAhead = schedDate.DayNumber - today.DayNumber;

                // Future or today — urgency by proximity of the upcoming shift
                if (daysAhead >= 0)
                    return daysAhead == 0 ? "urgent"
                         : daysAhead == 1 ? "urgent"
                         : daysAhead <= 2 ? "mid"
                         : "low";

                // Past date — urgency by how recently the change was logged
                return (DateTime.UtcNow - Timestamp).TotalHours switch
                {
                    <= 24 => "urgent",
                    <= 48 => "mid",
                    _     => "low"
                };
            }

            // Fallback
            return (DateTime.UtcNow - Timestamp).TotalHours switch
            {
                <= 24 => "urgent",
                <= 48 => "mid",
                _     => "low"
            };
        }
    }

    /// <summary>
    /// Human-readable reference: extracts the date from "empId:yyyy-MM-dd" EntityIds,
    /// or falls back to EntityType so raw GUIDs are never shown.
    /// </summary>
    private string FriendlyEntityRef
    {
        get
        {
            var parts = EntityId.Split(':');
            if (parts.Length == 2 && DateOnly.TryParse(parts[1], out var d))
                return d.ToString("MMM d, yyyy");
            // e.g. AppUser, Branch — just show the type, not a GUID
            return EntityType;
        }
    }

    public string Title => Action switch
    {
        "UpdateSchedule" => $"Schedule change — {FriendlyEntityRef}",
        "UpdateNote"     => $"Note updated — {FriendlyEntityRef}",
        _                => $"{Action} — {FriendlyEntityRef}"
    };

    public string Desc => Action == "UpdateSchedule"
        ? (OldValue is not null ? $"{OldValue} → {NewValue}" : NewValue ?? string.Empty)
        : NewValue ?? string.Empty;

    public string TimeDisplay => (DateTime.UtcNow - Timestamp) switch
    {
        var t when t.TotalMinutes < 60 => $"{(int)t.TotalMinutes}m ago",
        var t when t.TotalHours   < 24 => $"{(int)t.TotalHours}h ago",
        _                              => Timestamp.ToString("ddd h:mm tt")
    };
}
