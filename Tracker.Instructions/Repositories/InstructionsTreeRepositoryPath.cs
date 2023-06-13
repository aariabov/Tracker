using Microsoft.EntityFrameworkCore;
using Tracker.Instructions.Db;
using Tracker.Instructions.Db.Models;
using Tracker.Instructions.Interfaces;

namespace Tracker.Instructions.Repositories;

public class InstructionsTreeRepositoryPath : IInstructionsTreeRepository
{
    private readonly InstructionsDbContext _db;

    public InstructionsTreeRepositoryPath(InstructionsDbContext db)
    {
        _db = db;
    }

    // TODO: можно написать интеграционный тест
    public async Task<Instruction[]> GetTreeInstructionsAsync(int instructionId)
    {
        var treePath = await _db.Instructions
            .Where(i => i.Id == instructionId)
            .Select(i => i.TreePath)
            .SingleAsync();

        if (treePath is null)
        {
            throw new Exception($"Instruction with id '{instructionId}' has null tree path");
        }

        // tree path у рута будет, например "1", а у дочернего поручения, например "1/2/3"
        var index = treePath.IndexOf(TreePathsService.TreePathDelimiter);
        var rootId = index > 0 ? treePath.Substring(0, index) : treePath;

        // получаем дерево поручений из бд, делаем привязки parent/children - получаем плоский список с привязками
        var instructionTree = await _db.Instructions
            .Include(i => i.Creator)
            .Include(i => i.Executor)
            .Where(i => i.TreePath.StartsWith(rootId))
            .ToArrayAsync();

        return instructionTree;
    }

    public async Task<Instruction[]> GetUserInstructionsWithDescendantsAsync(string userId, int page, int perPage, Sort sort)
    {
        var sortedInstruction = _db.Instructions
            .Where(i => (i.CreatorId == userId && i.ParentId == null) || i.ExecutorId == userId);

        sortedInstruction = Helpers.AddSort(sortedInstruction, sort);

        var treePaths = sortedInstruction
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(i => i.TreePath);

        var instructions = _db.Instructions
            .Include(i => i.Creator)
            .Include(i => i.Executor)
            .Where(i => treePaths.Any(treePath => i.TreePath.Contains(treePath)));

        instructions = Helpers.AddSort(instructions, sort);

        return await instructions.ToArrayAsync();
    }
}
