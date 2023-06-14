using Tracker.Instructions.Db.Models;

namespace Tracker.Instructions;

public interface IInstructionsRepository
{
    /// <summary>
    /// Получить все поручения пользователя и их потомков
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Instruction[]> GetUserInstructionsWithDescendantsAsync(string userId, int page, int perPage, Sort sort);

    Task<int[]> GetRootInstructionIdsAsync();
    Task<int> GetRootInstructionCountAsync();
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
    Task<int> GetTotalUserInstructionsAsync(string userId);
    Task TruncateInstructions();
    Task UpdateSequence();
    Task SaveChangesAsync();
}
