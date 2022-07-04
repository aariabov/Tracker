using Tracker.Db.Models;

namespace Tracker.Instructions;

public interface IInstructionsRepository
{
    Task<Instruction[]> GetAllInstructionsAsync();
    Task<int[]> GetRootInstructionIdsAsync();
    Task<Instruction?> GetInstructionByIdAsync(int instructionId);
    
    /// <summary>
    /// Получает дерево на основе заданного способа работы с иерархиями
    /// </summary>
    /// <param name="instructionId"></param>
    /// <returns></returns>
    Task<Instruction> GetInstructionTreeAsync(int instructionId);
    
    /// <summary>
    /// Получает дерево на основе id/parentId
    /// </summary>
    /// <param name="instructionId"></param>
    /// <returns></returns>
    Task<Instruction> GetInstructionTreeByCteAsync(int instructionId);
    
    void CreateInstruction(Instruction instruction);
    void UpdateInstruction(Instruction instruction);
    Task<bool> IsInstructionExistsAsync(int instructionId);
    Task UpdateAllTreePathsToNullAsync();
    Task RecalculateAllInstructionsClosuresAsync();
    Task UpdateInstructionClosureAsync(int id, int? parentId);
    Task SaveChangesAsync();
}