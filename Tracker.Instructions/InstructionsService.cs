using Tracker.Db.Models;

namespace Tracker.Instructions;

public class InstructionsService : IInstructionsService
{
    public ExecStatus GetStatus(Instruction instruction)
    {
        if (!instruction.Children.Any())
            return GetStatusDespiteChildren(instruction);

        var anyChildInWork = AnyChildInWork(instruction);

        return anyChildInWork ? GetInWorkStatus(instruction) : GetStatusDespiteChildren(instruction);
    }
    
    public bool CanBeExecuted(Instruction instruction, string userId)
    {
        if (instruction.ExecDate is not null)
            return false;

        if (instruction.ExecutorId != userId)
            return false;
        
        if (!instruction.Children.Any())
            return true;
        
        return !AnyChildInWork(instruction);
    }
    
    public IEnumerable<Instruction> GetAllChildren(Instruction instruction)
    {
        var stack = new Stack<Instruction>();
        stack.Push(instruction);
        while(stack.Count > 0)
        {
            var current = stack.Pop();
            yield return current;
            foreach(var child in current.Children)
                stack.Push(child);
        }
        
        // реализация рекурсией - понятнее, но медленее и затратнее
        // var result = new List<Instruction> { instruction };
        // foreach (var child in instruction.Children)
        // {
        //     var children = GetAllChildren(child);
        //     result.AddRange(children);
        // }
        //
        // return result;
    }

    public Instruction GetRoot(Instruction instruction)
    {
        while (instruction.Parent is not null)
        {
            instruction = instruction.Parent;
        }
        
        return instruction;
    }
    
    public bool CanUserCreateChild(Instruction instruction, string userId, bool isUserBoss)
    {
        return instruction.ExecutorId == userId && isUserBoss && instruction.ExecDate is null;
    }

    private bool AnyChildInWork(Instruction instruction)
    {
        var inWorkStatuses = new List<ExecStatus> { ExecStatus.InWork, ExecStatus.InWorkOverdue };
        var anyChildInWork = instruction.Children.Any(c => inWorkStatuses.Contains(GetStatus(c)));
        return anyChildInWork;
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