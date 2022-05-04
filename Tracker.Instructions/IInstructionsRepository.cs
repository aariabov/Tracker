using Tracker.Db.Models;

namespace Tracker.Instructions;

public interface IInstructionsRepository
{
    Task<Instruction[]> GetAllInstructionsAsync();
    Task<Instruction?> GetInstructionByIdAsync(int instructionId);
    void CreateInstruction(Instruction instruction);
    void UpdateInstruction(Instruction instruction);
    Task<bool> IsInstructionExistsAsync(int instructionId);
    Task SaveChangesAsync();
}