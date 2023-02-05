using Tracker.Db.Models;

namespace Tracker.Instructions;

public class InstructionStatusService : IInstructionStatusService
{
    public ExecStatus GetStatus(Instruction instruction)
    {
        if (!instruction.Children.Any())
        {
            return GetStatusDespiteChildren(instruction);
        }

        var anyChildInWork = AnyChildInWork(instruction);

        return anyChildInWork ? GetInWorkStatus(instruction) : GetStatusDespiteChildren(instruction);
    }

    public bool AnyChildInWork(Instruction instruction)
    {
        var inWorkStatuses = new List<ExecStatus> { ExecStatus.InWork, ExecStatus.InWorkOverdue };
        var anyChildInWork = instruction.Children.Any(c => inWorkStatuses.Contains(GetStatus(c)));
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
