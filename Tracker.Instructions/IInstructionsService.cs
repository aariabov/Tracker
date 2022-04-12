using Tracker.Db.Models;

namespace Tracker.Instructions;

public interface IInstructionsService
{
    ExecStatus GetStatus(Instruction instruction);
    bool CanBeExecuted(Instruction instruction, string userId);
    bool CanUserCreateChild(Instruction instruction, string userId, bool isUserBoss);
    Instruction GetRoot(Instruction instruction);
    IEnumerable<Instruction> GetAllChildren(Instruction instruction);
}