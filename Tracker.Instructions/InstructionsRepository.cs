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
}