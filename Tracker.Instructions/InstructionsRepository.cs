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

    public InstructionsRepository(AppDbContext db, IInstructionsTreeRepository treeRepository)
    {
        _db = db;
        _treeRepository = treeRepository;
        _cteRepository = new InstructionsTreeRepositoryCte(db);
    }

    public async Task<Instruction[]> GetAllInstructionsAsync()
    {
        return await _db.Instructions
            .Include(i => i.Creator)
            .Include(i => i.Executor)
            .ToArrayAsync();
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

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
}