using Microsoft.EntityFrameworkCore;
using Tracker.Db;
using Tracker.Db.Models;
using Tracker.Instructions.Interfaces;

namespace Tracker.Instructions.Repositories;

public class InstructionsTreeRepositoryCte : IInstructionsTreeRepository
{
    private readonly AppDbContext _db;

    public InstructionsTreeRepositoryCte(AppDbContext db)
    {
        _db = db;
    }
    
    // TODO: можно написать интеграционный тест
    public async Task<Instruction[]> GetTreeInstructionsAsync(int instructionId)
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
        return await _db.Instructions.FromSqlInterpolated(cte)
            .Include(i => i.Creator)
            .Include(i => i.Executor)
            .ToArrayAsync();
    }

    public async Task<Instruction[]> GetUserInstructionsWithDescendantsAsync(string userId)
    {
        FormattableString cte = @$"
WITH RECURSIVE r AS (
   SELECT instructions.*, 1 AS level
   FROM instructions
   WHERE creator_id = {userId} OR executor_id = {userId}

   UNION ALL

   SELECT org.*, r.level + 1 AS level
   FROM instructions org
      JOIN r ON org.parent_id = r.id
)

SELECT * FROM r";
        
        // получаем дерево поручений из бд, делаем привязки parent/children - получаем плоский список с привязками
        return await _db.Instructions.FromSqlInterpolated(cte)
            .Include(i => i.Creator)
            .Include(i => i.Executor)
            .ToArrayAsync();
    }

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