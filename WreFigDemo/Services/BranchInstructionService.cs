using Microsoft.EntityFrameworkCore;
using WreFigDemo.Data;
using WreFigDemo.Models;
using WreFigDemo.Models.Entities;
using WreFigDemo.Models.ViewModels;

namespace WreFigDemo.Services;

public class BranchInstructionService(IDbContextFactory<AppDbContext> dbFactory) : IBranchInstructionService
{
    public async Task<BranchInstructionsVm> GetForBranchAsync(int branchId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var rows = await db.BranchInstructions
            .Where(i => i.BranchId == branchId)
            .OrderBy(i => i.Position)
            .ThenBy(i => i.SortOrder)
            .AsNoTracking()
            .ToListAsync();

        var vm = new BranchInstructionsVm { BranchId = branchId };

        foreach (var def in InstructionSections.All)
        {
            vm.Sections.Add(new BranchInstructionSectionVm
            {
                SectionKey   = def.Key,
                SectionTitle = def.Title,
                Position     = def.Position,
                Lines        = rows
                    .Where(r => r.Section == def.Key)
                    .Select(r => new BranchInstructionLineVm
                    {
                        Id            = r.Id,
                        Content       = r.Content,
                        IsHighlighted = r.IsHighlighted,
                        SortOrder     = r.SortOrder,
                        UpdatedByName = r.UpdatedByName,
                        UpdatedAt     = r.UpdatedAt,
                    })
                    .ToList()
            });
        }

        return vm;
    }

    public async Task SaveAllAsync(BranchInstructionsVm vm, string userId, string userName)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        // Delete-and-reinsert: dataset is tiny (max ~50 lines per branch)
        var existing = await db.BranchInstructions
            .Where(i => i.BranchId == vm.BranchId)
            .ToListAsync();

        db.BranchInstructions.RemoveRange(existing);

        var now = DateTime.UtcNow;
        foreach (var sec in vm.Sections)
        {
            var def = InstructionSections.Find(sec.SectionKey);
            if (def is null) continue;

            int order = 0;
            foreach (var line in sec.Lines.Where(l => !l.IsDeleted && !string.IsNullOrWhiteSpace(l.Content)))
            {
                db.BranchInstructions.Add(new BranchInstruction
                {
                    BranchId      = vm.BranchId,
                    Position      = def.Position,
                    Section       = def.Key,
                    Content       = line.Content.Trim(),
                    IsHighlighted = line.IsHighlighted,
                    SortOrder     = order++,
                    UpdatedAt     = now,
                    UpdatedById   = userId,
                    UpdatedByName = userName,
                });
            }
        }

        await db.SaveChangesAsync();
    }
}
