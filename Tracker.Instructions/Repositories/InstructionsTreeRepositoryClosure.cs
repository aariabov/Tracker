using Microsoft.EntityFrameworkCore;
using Tracker.Db;
using Tracker.Db.Models;
using Tracker.Instructions.Interfaces;

namespace Tracker.Instructions.Repositories;

public class InstructionsTreeRepositoryClosure : IInstructionsTreeRepository
{
    private readonly AppDbContext _db;

    public InstructionsTreeRepositoryClosure(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<Instruction[]> GetTreeInstructionsAsync(int instructionId)
    {
        var rootId = await GetTreeRootId(instructionId);
        
        return await _db.Instructions
            .Include(i => i.Creator)
            .Include(i => i.Executor)
            .Where(i => _db.InstructionsClosures.Where(ic => ic.ParentId == rootId).Select(ic => ic.Id).Contains(i.Id))
            .ToArrayAsync();
    }

    public async Task<Instruction[]> GetUserInstructionsWithDescendantsAsync(string userId)
    {
        var ids = _db.Instructions
            .Where(i => i.CreatorId == userId || i.ExecutorId == userId)
            .Select(i => i.Id);
        
        return await _db.Instructions
            .Include(i => i.Creator)
            .Include(i => i.Executor)
            .Where(i => _db.InstructionsClosures.Where(ic => ids.Contains(ic.ParentId)).Select(ic => ic.Id).Contains(i.Id))
            .ToArrayAsync();
    }

    public async Task RecalculateAllInstructionsClosuresAsync()
    {
        await DeleteFromInstructionsClosuresAsync();
        
        var sql = @"
WITH RECURSIVE cte AS
(
    SELECT
        id AS parent_id,
        id,
        0 AS depth
    FROM instructions

    UNION ALL

    SELECT
        cte.parent_id,
        i.id,
        cte.depth + 1 AS depth
    FROM instructions AS i
    JOIN cte ON i.parent_id = cte.id
)
INSERT INTO instructions_closures
SELECT * FROM cte order by parent_id;";
        
        await _db.Database.ExecuteSqlRawAsync(sql);
    }

    public async Task UpdateInstructionClosureAsync(int id, int? parentId)
    {
        if (!parentId.HasValue)
        {
            FormattableString sql = $"insert into instructions_closures (parent_id, id, depth) values ({id}, {id}, 0)";
            await _db.Database.ExecuteSqlInterpolatedAsync(sql);
            return;
        }

        FormattableString withParentsSql = $@"
insert into instructions_closures (parent_id, id, depth)
select parent_id, {id}, depth + 1 from instructions_closures where id = {parentId.Value}
union all select {id}, {id}, 0";
        await _db.Database.ExecuteSqlInterpolatedAsync(withParentsSql);
    }

    private async Task DeleteFromInstructionsClosuresAsync()
    {
        await _db.Database.ExecuteSqlRawAsync("delete from instructions_closures");
    }

    private async Task<int> GetTreeRootId(int instructionId)
    {
        var maxDepth = await _db.InstructionsClosures
            .Where(c => c.Id == instructionId)
            .MaxAsync(c => c.Depth);
        
        var closureRow = await _db.InstructionsClosures.SingleAsync(c => c.Id == instructionId && c.Depth == maxDepth);
        return closureRow.ParentId;
    }
}