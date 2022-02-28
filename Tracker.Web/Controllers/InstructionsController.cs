using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Db;
using Tracker.Web.Domain;
using Tracker.Web.ViewModels;

namespace Tracker.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstructionsController : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<InstructionVm>> GetAllInstructions()
    {
        await using var db = new AppDbContext();
        var allInstructions = await db.Instructions
            .Include(i => i.Creator)
            .Include(i => i.Executor)
            .ToArrayAsync();
        
        var instructionVms = InstructionVm.CreateCollection(allInstructions);
        return instructionVms;
    }
    
    [HttpPost]
    public async Task<Instruction> CreateInstruction([FromBody]Instruction instruction)
    {
        await using var db = new AppDbContext();
        db.Instructions.Add(instruction);
        await db.SaveChangesAsync();
        return instruction;
    }
}