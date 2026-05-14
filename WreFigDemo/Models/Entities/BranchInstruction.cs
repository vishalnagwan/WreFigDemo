namespace WreFigDemo.Models.Entities;

public enum InstructionPosition { Top = 0, Bottom = 1 }

/// <summary>
/// One content line within a named section of a branch's instruction sheet.
/// Top position displays above the schedule grid (black header);
/// Bottom position displays below the grid (yellow header).
/// </summary>
public class BranchInstruction
{
    public int    Id            { get; set; }
    public int    BranchId      { get; set; }
    public Branch Branch        { get; set; } = null!;

    /// <summary>Top = above the schedule grid; Bottom = below the grid.</summary>
    public InstructionPosition Position  { get; set; }

    /// <summary>Section key, e.g. "ServiceLimitations". Matches InstructionSections.All.</summary>
    public string Section       { get; set; } = string.Empty;

    /// <summary>The instruction text shown to users.</summary>
    public string Content       { get; set; } = string.Empty;

    /// <summary>When true, line displays red with a blinking animation for urgent attention.</summary>
    public bool   IsHighlighted { get; set; }

    public int    SortOrder     { get; set; }

    public DateTime UpdatedAt     { get; set; } = DateTime.UtcNow;
    public string   UpdatedById   { get; set; } = string.Empty;
    public string   UpdatedByName { get; set; } = string.Empty;
}
