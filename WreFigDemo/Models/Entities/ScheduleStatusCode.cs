namespace WreFigDemo.Models.Entities;

public class ScheduleStatusCode
{
    public string Code           { get; set; } = string.Empty;
    public string Label          { get; set; } = string.Empty;
    public string CssClass       { get; set; } = string.Empty;
    public string Description    { get; set; } = string.Empty;
    public int    SortOrder      { get; set; }
    public bool   ShowInPaintBar { get; set; }
    public bool   ShowInPicker   { get; set; }
}
