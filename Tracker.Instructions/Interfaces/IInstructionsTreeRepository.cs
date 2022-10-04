using Tracker.Db.Models;

namespace Tracker.Instructions.Interfaces;

public interface IInstructionsTreeRepository
{
    Task<Instruction[]> GetTreeInstructionsAsync(int instructionId);
    Task<Instruction[]> GetUserInstructionsWithDescendantsAsync(string userId, int page, int perPage, Sort sort);
}