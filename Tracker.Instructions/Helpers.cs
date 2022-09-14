using Tracker.Db.Models;

namespace Tracker.Instructions;

public static class Helpers
{
    public static string GetString(this ExecStatus status)
    {
        switch (status)
        {
            case ExecStatus.InWork: return "В работе";
            case ExecStatus.InWorkOverdue: return "В работе просрочено";
            case ExecStatus.Completed: return "Выполнено в срок";
            case ExecStatus.CompletedOverdue: return "Выполнено с нарушением срока";
            default: throw new Exception("Unknown ExecStatus");
        }
    }

    public static IEnumerable<Instruction> GetAllChildren(Instruction instruction)
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
}