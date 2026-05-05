namespace WreFigDemo.Models.ViewModels;

public class AlertVm
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime Timestamp { get; set; }

    public bool IsUnread { get; set; } = true;

    public string Urgency => (DateTime.UtcNow - Timestamp).TotalHours switch
    {
        <= 24 => "urgent",
        <= 48 => "mid",
        _     => "low"
    };

    public string Title => $"{Action} — {EntityId}";
    public string Desc  => OldValue is not null
        ? $"{OldValue} → {NewValue}"
        : NewValue ?? string.Empty;

    public string TimeDisplay => Timestamp > DateTime.UtcNow.AddHours(-1)
        ? $"{(int)(DateTime.UtcNow - Timestamp).TotalMinutes} min ago"
        : $"{(int)(DateTime.UtcNow - Timestamp).TotalHours}h ago";
}
