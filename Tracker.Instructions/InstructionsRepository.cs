using Microsoft.EntityFrameworkCore;
using Tracker.Instructions.Db;
using Tracker.Instructions.Db.Models;
using Tracker.Instructions.Interfaces;
using Tracker.Instructions.Repositories;

namespace Tracker.Instructions;

public class InstructionsRepository
{
    private readonly InstructionsDbContext _db;
    private readonly IInstructionsTreeRepository _treeRepository;

    // CTE Repository нужен для пересчета данных для быстрой работы с иерархиями
    // для способа Enumeration Paths надо пересчитать tree_path
    // для способа Closure надо пересчитать closure table
    // для способа Nested sets надо пересчитать все коэффициенты
    private readonly InstructionsTreeRepositoryCte _cteRepository;

    // нужен, чтобы держать closure table в актуальном состоянии, независимо от способа работы с иерархиями,
    // чтобы можно было переключать способы без полного пересчета
    private readonly InstructionsTreeRepositoryClosure _closureRepository;

    public InstructionsRepository(InstructionsDbContext db, IInstructionsTreeRepository treeRepository)
    {
        _db = db;
        _treeRepository = treeRepository;
        _cteRepository = new InstructionsTreeRepositoryCte(db);
        _closureRepository = new InstructionsTreeRepositoryClosure(db);
    }

    public async Task<Instruction[]> GetUserInstructionsWithDescendantsAsync(string userId, int page, int perPage, Sort sort)
    {
        return await _treeRepository.GetUserInstructionsWithDescendantsAsync(userId, page, perPage, sort);
    }

    public async Task<int[]> GetRootInstructionIdsAsync()
    {
        return await _db.Instructions
            .Where(i => i.ParentId == null)
            .Select(i => i.Id)
            .ToArrayAsync();
    }

    public async Task<int[]> GetInWorkRootInstructionIdsAsync()
    {
        return await _db.Instructions
            .Where(i => i.ParentId == null
                        && (i.StatusId == (long)ExecStatus.InWork || i.StatusId == (long)ExecStatus.InWorkOverdue))
            .Select(i => i.Id)
            .ToArrayAsync();
    }

    public async Task<int[]> GetInstructionIdsAsync()
    {
        return await _db.Instructions
            .Select(i => i.Id)
            .ToArrayAsync();
    }

    public async Task<IEnumerable<int>> GetReCalcStatusRootInstructionIds()
    {
        return await _treeRepository.GetReCalcStatusRootInstructionIds();
    }

    public async Task<int> GetRootInstructionCountAsync()
    {
        return await _db.Instructions.CountAsync(i => i.ParentId == null);
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

    public async Task UpdateAllStatusesToNullAsync()
    {
        await _db.Database.ExecuteSqlRawAsync("update instructions set status_id = null");
        // EF мог закешировать статусы
        _db.ChangeTracker.Clear();
    }

    public async Task RecalculateAllInstructionsClosuresAsync()
    {
        await _closureRepository.RecalculateAllInstructionsClosuresAsync();
    }

    public async Task UpdateInstructionClosureAsync(int id, int? parentId)
    {
        await _closureRepository.UpdateInstructionClosureAsync(id, parentId);
    }

    public async Task<int> GetTotalUserInstructionsAsync(string userId)
    {
        return await _db.Instructions
            .CountAsync(i => (i.CreatorId == userId && i.ParentId == null) || i.ExecutorId == userId);
    }

    public Task TruncateInstructions()
    {
        return _db.Database.ExecuteSqlRawAsync("TRUNCATE instructions RESTART IDENTITY CASCADE;");
    }

    public async Task UpdateSequence()
    {
        var number = await _db.Instructions.CountAsync();
        await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT setval('instructions_id_seq', {number});");
    }

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
}
