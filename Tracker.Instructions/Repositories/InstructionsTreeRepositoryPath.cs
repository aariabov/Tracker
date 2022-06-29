using Microsoft.EntityFrameworkCore;
using Tracker.Db;
using Tracker.Db.Models;
using Tracker.Instructions.Interfaces;

namespace Tracker.Instructions.Repositories;

public class InstructionsTreeRepositoryPath : IInstructionsTreeRepository
{
    private readonly AppDbContext _db;

    public InstructionsTreeRepositoryPath(AppDbContext db)
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
            throw new Exception($"Instruction with id '{instructionId}' has null tree path");
        
        // tree path у рута будет, например "1", а у дочернего поручения, например "1/2/3"
        var index = treePath.IndexOf(InstructionsService.TreePathDelimiter);
        var rootId = index > 0 ? treePath.Substring(0, index) : treePath;

        // получаем дерево поручений из бд, делаем привязки parent/children - получаем плоский список с привязками
        var instructionTree = await _db.Instructions
            .Include(i => i.Creator)
            .Include(i => i.Executor)
            .Where(i => i.TreePath.StartsWith(rootId))
            .ToArrayAsync();

        return instructionTree;
    }
}