using Tracker.Instructions.Db.Models;

namespace Tracker.Instructions;

public interface IInstructionStatusService
{
    ExecStatus GetStatus(Instruction instruction);
    bool AnyChildInWork(Instruction instruction);

    /// <summary>
    /// Получить макс дату исполнения, если все потомки исполнены, в противном случае null
    /// </summary>
    /// <param name="instruction"></param>
    /// <returns></returns>
    DateTime? GetMaxChildrenExecDate(Instruction instruction);
}
