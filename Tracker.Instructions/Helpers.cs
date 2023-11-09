using Tracker.Instructions.Db.Models;

namespace Tracker.Instructions;

public static class Helpers
{
    public static string GetString(this ExecStatus status)
    {
        switch (status)
        {
            case ExecStatus.InWork:
                return "В работе";
            case ExecStatus.InWorkOverdue:
                return "В работе просрочено";
            case ExecStatus.Completed:
                return "Выполнено в срок";
            case ExecStatus.CompletedOverdue:
                return "Выполнено с нарушением срока";
            default:
                throw new Exception("Unknown ExecStatus");
        }
    }

    public static IEnumerable<Instruction> GetAllChildren(Instruction instruction)
    {
        var stack = new Stack<Instruction>();
        stack.Push(instruction);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            yield return current;
            foreach (var child in current.Children)
            {
                stack.Push(child);
            }
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

    internal static Sort GetSort(string sort) =>
        sort switch
        {
            "name" => Sort.NameAsc,
            "-name" => Sort.NameDesc,
            "creatorName" => Sort.CreatorAsc,
            "-creatorName" => Sort.CreatorDesc,
            "executorName" => Sort.ExecutorAsc,
            "-executorName" => Sort.ExecutorDesc,
            "deadline" => Sort.DeadlineAsc,
            "-deadline" => Sort.DeadlineDesc,
            "execDate" => Sort.ExecDateAsc,
            "-execDate" => Sort.ExecDateDesc,
            _ => throw new ArgumentException("Invalid sort string", nameof(sort)),
        };

    internal static IOrderedQueryable<Instruction> AddSort(IQueryable<Instruction> instructions, Sort sort) =>
        sort switch
        {
            Sort.NameAsc => instructions.OrderBy(i => i.Name),
            Sort.NameDesc => instructions.OrderByDescending(i => i.Name),
            Sort.CreatorAsc => instructions.OrderBy(i => i.Creator.UserName),
            Sort.CreatorDesc => instructions.OrderByDescending(i => i.Creator.UserName),
            Sort.ExecutorAsc => instructions.OrderBy(i => i.Executor.UserName),
            Sort.ExecutorDesc => instructions.OrderByDescending(i => i.Executor.UserName),
            Sort.DeadlineAsc => instructions.OrderBy(i => i.Deadline),
            Sort.DeadlineDesc => instructions.OrderByDescending(i => i.Deadline),
            Sort.ExecDateAsc => instructions.OrderBy(i => i.ExecDate),
            Sort.ExecDateDesc => instructions.OrderByDescending(i => i.ExecDate),
            _ => throw new ArgumentException("Unknown sort", nameof(sort))
        };
}
