using Tracker.Instructions.Db.Models;

namespace Tracker.Instructions;

public class InstructionStatusService
{
    public void ReCalcStatus(Instruction instruction)
    {
        if (!instruction.Children.Any())
        {
            var statusDespiteChildren = GetStatusDespiteChildren(instruction);
            instruction.StatusId = (int?)statusDespiteChildren;
            return;
        }

        var anyChildInWork = AnyChildInWork(instruction);

        var status = anyChildInWork ? GetInWorkStatus(instruction) : GetStatusDespiteChildren(instruction);
        instruction.StatusId = (int?)status;
    }

    public bool AnyChildInWork(Instruction instruction)
    {
        var inWorkStatuses = new List<int?> { (int?)ExecStatus.InWork, (int?)ExecStatus.InWorkOverdue };
        var anyChildInWork = false;
        foreach (var child in instruction.Children)
        {
            ReCalcStatus(child);
            if (inWorkStatuses.Contains(child.StatusId))
            {
                anyChildInWork = true;
            }
        }
        return anyChildInWork;
    }

    public DateTime? GetMaxChildrenExecDate(Instruction instruction)
    {
        var instructions = Helpers.GetAllChildren(instruction).Where(i => i.Id != instruction.Id).ToArray();
        if (instructions.Any(i => i.ExecDate is null))
        {
            return null;
        }

        return instructions.Max(i => i.ExecDate);
    }

    private ExecStatus GetStatusDespiteChildren(Instruction instruction)
    {
        return instruction.ExecDate is null
            ? GetInWorkStatus(instruction)
            : GetCompletedStatus(instruction);
    }

    private ExecStatus GetInWorkStatus(Instruction instruction)
    {
        return DateTime.UtcNow.Date > instruction.Deadline ? ExecStatus.InWorkOverdue : ExecStatus.InWork;
    }

    private ExecStatus GetCompletedStatus(Instruction instruction)
    {
        return instruction.ExecDate > instruction.Deadline
            ? ExecStatus.CompletedOverdue
            : ExecStatus.Completed;
    }
}
