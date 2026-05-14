namespace WreFigDemo.Services;

/// <summary>
/// Singleton in-process event bus.
/// When a branch's instructions are saved, the broadcast fires and every Blazor circuit
/// currently viewing that branch reloads its instruction panels immediately —
/// no page refresh required for dispatchers or supervisors.
/// </summary>
public class BranchInstructionBroadcastService
{
    public event Func<int, string, Task>? InstructionsUpdated;

    public async Task BroadcastAsync(int branchId, string updatedByName)
    {
        var handler = InstructionsUpdated;
        if (handler is null) return;

        foreach (var d in handler.GetInvocationList().Cast<Func<int, string, Task>>())
        {
            try   { await d(branchId, updatedByName); }
            catch { /* circuit may have been disposed — ignore */ }
        }
    }
}
