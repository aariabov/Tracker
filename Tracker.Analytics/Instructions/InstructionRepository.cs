using Tracker.Analytics.Db;
using Tracker.Analytics.Db.Models;

namespace Tracker.Analytics.Instructions;

public class InstructionRepository
{
    private readonly AnalyticsDbContext _db;

    public InstructionRepository(AnalyticsDbContext db)
    {
        _db = db;
    }

    public void UpdateInstruction(Instruction instruction)
    {
        _db.Instructions.Update(instruction);
    }

    public void InsertInstruction(Instruction instruction)
    {
        _db.Instructions.Add(instruction);
    }

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
}
