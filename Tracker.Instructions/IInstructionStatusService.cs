using Tracker.Db.Models;

namespace Tracker.Instructions;

public interface IInstructionStatusService
{
    ExecStatus GetStatus(Instruction instruction);
    bool AnyChildInWork(Instruction instruction);
}