using WreFigDemo.Models.Entities;

namespace WreFigDemo.Models.ViewModels;

public class BranchInstructionLineVm
{
    public int      Id            { get; set; }   // 0 = new unsaved row
    public string   Content       { get; set; } = string.Empty;
    public bool     IsHighlighted { get; set; }
    public int      SortOrder     { get; set; }
    public string   UpdatedByName { get; set; } = string.Empty;
    public DateTime UpdatedAt     { get; set; }

    /// <summary>Editor-only: marks row for removal on next save.</summary>
    public bool     IsDeleted     { get; set; }
}

public class BranchInstructionSectionVm
{
    public string                        SectionKey   { get; set; } = string.Empty;
    public string                        SectionTitle { get; set; } = string.Empty;
    public string                        ShortTitle   { get; set; } = string.Empty;
    public InstructionPosition           Position     { get; set; }
    public List<BranchInstructionLineVm> Lines        { get; set; } = [];

    public bool HasContent      => Lines.Any(l => !l.IsDeleted && !string.IsNullOrWhiteSpace(l.Content));
    public bool HasUrgent       => Lines.Any(l => !l.IsDeleted && !string.IsNullOrWhiteSpace(l.Content) && l.IsHighlighted);
    public int  ActiveLineCount => Lines.Count(l => !l.IsDeleted && !string.IsNullOrWhiteSpace(l.Content));
}

public class BranchInstructionsVm
{
    public int                              BranchId { get; set; }
    public List<BranchInstructionSectionVm> Sections { get; set; } = [];

    public bool HasTopContent    => Sections.Any(s => s.Position == InstructionPosition.Top    && s.HasContent);
    public bool HasBottomContent => Sections.Any(s => s.Position == InstructionPosition.Bottom && s.HasContent);
}
