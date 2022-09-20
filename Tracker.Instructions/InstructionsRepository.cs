using Microsoft.EntityFrameworkCore;
using Tracker.Db;
using Tracker.Db.Models;
using Tracker.Instructions.Interfaces;
using Tracker.Instructions.Repositories;

namespace Tracker.Instructions;

public class InstructionsRepository : IInstructionsRepository
{
    private readonly AppDbContext _db;
    private readonly IInstructionsTreeRepository _treeRepository;
    
    // CTE Repository нужен для пересчета данных для быстрой работы с иерархиями
    // для способа Enumeration Paths надо пересчитать tree_path
    // для способа Closure надо пересчитать closure table
    // для способа Nested sets надо пересчитать все коэффициенты
    private readonly InstructionsTreeRepositoryCte _cteRepository;
    
    // нужен, чтобы держать closure table в актуальном состоянии, независимо от способа работы с иерархиями,
    // чтобы можно было переключать способы без полного пересчета
    private readonly InstructionsTreeRepositoryClosure _closureRepository;

    public InstructionsRepository(AppDbContext db, IInstructionsTreeRepository treeRepository)
    {
        _db = db;
        _treeRepository = treeRepository;
        _cteRepository = new InstructionsTreeRepositoryCte(db);
        _closureRepository = new InstructionsTreeRepositoryClosure(db);
    }

    public async Task<Instruction[]> GetUserInstructionsWithDescendantsAsync(string userId)
    {
        return await _treeRepository.GetUserInstructionsWithDescendantsAsync(userId);
    }

    public async Task<int[]> GetRootInstructionIdsAsync()
    {
        return await _db.Instructions
            .Where(i => i.ParentId == null)
            .Select(i => i.Id)
            .ToArrayAsync();
    }

    public async Task<Instruction?> GetInstructionByIdAsync(int instructionId)
    {
        return await _db.Instructions.SingleOrDefaultAsync(i => i.Id == instructionId);
    }

    public async Task<Instruction> GetInstructionTreeAsync(int instructionId)
    {
        var instructionTree = await _treeRepository.GetTreeInstructionsAsync(instructionId);
        
        // возвращаем поручение, которое запрашивали, но уже со всеми parent/children
        return instructionTree.Single(i => i.Id == instructionId);
    }

    public async Task<Instruction> GetInstructionTreeByCteAsync(int instructionId)
    {
        var instructionTree = await _cteRepository.GetTreeInstructionsAsync(instructionId);
        return instructionTree.Single(i => i.Id == instructionId);
    }

    public void CreateInstruction(Instruction instruction)
    {
        _db.Instructions.Add(instruction);
    }

    public void UpdateInstruction(Instruction instruction)
    {
        _db.Instructions.Update(instruction);
    }

    public async Task<bool> IsInstructionExistsAsync(int instructionId)
    {
        return await _db.Instructions.AnyAsync(u => u.Id == instructionId);
    }

    public async Task UpdateAllTreePathsToNullAsync()
    {
        await _db.Database.ExecuteSqlRawAsync("update instructions set tree_path = null");
    }

    public async Task RecalculateAllInstructionsClosuresAsync()
    {
        await _closureRepository.RecalculateAllInstructionsClosuresAsync();
    }

    public async Task UpdateInstructionClosureAsync(int id, int? parentId)
    {
        await _closureRepository.UpdateInstructionClosureAsync(id, parentId);
    }

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
}