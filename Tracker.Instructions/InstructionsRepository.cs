using Microsoft.EntityFrameworkCore;
using Tracker.Db;
using Tracker.Db.Models;

namespace Tracker.Instructions;

public class InstructionsRepository : IInstructionsRepository
{
    private readonly AppDbContext _db;

    public InstructionsRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Instruction[]> GetAllInstructionsAsync()
    {
        return await _db.Instructions
            .Include(i => i.Creator)
            .Include(i => i.Executor)
            .ToArrayAsync();
    }

    public async Task<Instruction?> GetInstructionByIdAsync(int instructionId)
    {
        return await _db.Instructions.SingleOrDefaultAsync(i => i.Id == instructionId);
    }

    public async Task<Instruction> GetInstructionTreeAsync(int instructionId)
    {
        var rootId = await GetTreeRootId(instructionId);

        FormattableString cte = @$"
WITH RECURSIVE r AS (
   SELECT instructions.*, 1 AS level
   FROM instructions
   WHERE id = {rootId}

   UNION ALL

   SELECT org.*, r.level + 1 AS level
   FROM instructions org
      JOIN r ON org.parent_id = r.id
)

SELECT * FROM r";
        
        // получаем дерево поручений из бд, делаем привязки parent/children - получаем плоский список с привязками
        var instructionTree = await _db.Instructions.FromSqlInterpolated(cte)
            .Include(i => i.Creator)
            .Include(i => i.Executor)
            .ToArrayAsync();

        // возвращаем поручение, которое запрашивали, но уже со всеми parent/children
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

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

    private async Task<int> GetTreeRootId(int instructionId)
    {
        FormattableString cte = @$"
WITH RECURSIVE r AS (
   SELECT id, name, parent_id, 1 AS level
   FROM instructions
   WHERE id = {instructionId}

   UNION ALL

   SELECT i.id, i.name, i.parent_id, r.level + 1 AS level
   FROM instructions i
      JOIN r ON i.id = r.parent_id
)

SELECT * FROM r ORDER BY level desc LIMIT 1";
        
        var rootInstructionId = await _db.Instructions.FromSqlInterpolated(cte)
            .Select(i => i.Id)
            .SingleAsync();

        return rootInstructionId;
    }
}