using Tracker.Common;

namespace Tracker.Db.Models;

public class Instruction
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string CreatorId { get; set; }
    public string ExecutorId { get; set; }
    public User? Creator { get; set; }
    public User? Executor { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime? ExecDate { get; set; }
    public Instruction? Parent { get; set; }
    public ICollection<Instruction> Children { get; set; } = new List<Instruction>();

    // TODO: убрать логику
    public ExecStatus Status
    {
        get
        {
            if (!Children.Any())
                return GetStatusDespiteChildren;

            var anyChildInWork = AnyChildInWork();

            return anyChildInWork ? GetInWorkStatus : GetStatusDespiteChildren;
        }
    }

    public void SetExecDate(DateTime execDate, string userId)
    {
        if (CanBeExecuted(userId))
            ExecDate = execDate;
    }

    public bool CanBeExecuted(string userId)
    {
        if (ExecDate is not null)
            return false;

        if (ExecutorId != userId)
            return false;
        
        if (!Children.Any())
            return true;
        
        return !AnyChildInWork();
    }

    public bool CanUserCreateChild(string userId, bool isUserBoss)
    {
        return ExecutorId == userId && isUserBoss && ExecDate is null;
    }

    public Instruction GetRoot()
    {
        return Parent is null ? this : Parent.GetRoot();
    }

    public IEnumerable<Instruction> GetAllChildren()
    {
        var result = new List<Instruction> { this };
        foreach (var child in Children)
        {
            var children = child.GetAllChildren();
            result.AddRange(children);
        }
        
        return result;
        
        // реализация на стеке - должен быстрей работать и потреблять меньше памяти
        // var result = new List<Instruction>();
        // var stack = new Stack<Instruction>();
        // stack.Push(this);
        // while(stack.Count > 0)
        // {
        //     var current = stack.Pop();
        //     result.Add(current);
        //     foreach(var child in current.Children)
        //         stack.Push(child);
        // }
        // return result;
    }

    public override string ToString()
    {
        return $"{Id} {Name}";
    }

    private bool AnyChildInWork()
    {
        var inWorkStatuses = new List<ExecStatus> { ExecStatus.InWork, ExecStatus.InWorkOverdue };
        var anyChildInWork = Children.Any(c => inWorkStatuses.Contains(c.Status));
        return anyChildInWork;
    }

    private ExecStatus GetStatusDespiteChildren => ExecDate is null ? GetInWorkStatus : GetCompletedStatus;
    private ExecStatus GetInWorkStatus => DateTime.Today > Deadline ? ExecStatus.InWorkOverdue : ExecStatus.InWork;
    private ExecStatus GetCompletedStatus => ExecDate > Deadline ? ExecStatus.CompletedOverdue : ExecStatus.Completed;
}