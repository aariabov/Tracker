using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Db;
using Tracker.Web.Domain;
using Tracker.Web.RequestModels;
using Tracker.Web.ViewModels;

namespace Tracker.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstructionsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;
    
    public InstructionsController(AppDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<InstructionVm>))]
    public async Task<ActionResult<IEnumerable<InstructionVm>>> GetUserInstructions()
    {
        var userId = GetCurrentUserId();
        var allInstructions = await GetAllInstructions();
        var userInstructions = allInstructions
            .Where(i => i.CreatorId == userId || i.ExecutorId == userId);
        
        var isUserBoss = await _userManager.Users.AnyAsync(u => u.BossId == userId);

        var instructionVms = userInstructions.Select(instruction =>
        {
            var canCreateChild = instruction.CanUserCreateChild(userId, isUserBoss);
            var canBeExecuted = instruction.CanBeExecuted(userId);
            return InstructionVm.Create(instruction, canCreateChild, canBeExecuted);
        });
        
        return Ok(instructionVms);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<InstructionVm>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<InstructionVm>>> GetTreeInstruction(int id)
    {
        // Функции и процедуры не поддерживаются в SQLite
        // SQL не может содержать связанные данные, но можно потом добавить Include
        // К CTE нельзя добавить экстеншены типа Include, можно только к запросам, которые начинаются с select
        // Можно рекурсивно вытягивать всех детей, но будет много запросов
        // Можно вытянуть все записи (все дети уже будут привязаны), взять родительское поручение,
        // и рекурсивно преобразовать в плоский список - подойдет только для маленьких иерархий
        
        // Итого
        // пока можно просто вытянуть все записи
        // потом, на нормальной базе можно создать функцию и использовать CTE
        // если нужна будет производительность то использовать Closure, Enumeration paths или Nested sets - 
        // во всех случаях будет гемор с пересчетом, но Closure понятнее
        
        var allInstructions = await GetAllInstructions();
        var instruction = allInstructions.SingleOrDefault(i => i.Id == id);
        if (instruction is null)
            return NotFound();
        
        var rootInstruction = instruction.GetRoot();
        var instructions = rootInstruction.GetAllChildren();
        var result = InstructionVm.CreateCollection(instructions);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Instruction))]
    public async Task<ActionResult<Instruction>> CreateInstruction([FromBody]InstructionRm instruction)
    {
        var userId = GetCurrentUserId();
        // TODO: добавить валидацию
        var newInstruction = new Instruction
        {
            Name = instruction.Name,
            CreatorId = userId,
            ExecutorId = instruction.ExecutorId,
            ParentId = instruction.ParentId,
            Deadline = instruction.Deadline
        };
        
        _db.Instructions.Add(newInstruction);
        await _db.SaveChangesAsync();
        return Ok(newInstruction);
    }
    
    [HttpPost("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetExecDate(int id, [FromBody] DateTime execDate)
    {
        var userId = GetCurrentUserId();
        var allInstructions = await GetAllInstructions();
        var instruction = allInstructions.SingleOrDefault(i => i.Id == id);
        if(instruction is null)
            return NotFound();
        
        instruction.SetExecDate(execDate, userId);
        await _db.SaveChangesAsync();
        return Ok();
    }

    private async Task<Instruction[]> GetAllInstructions()
    {
        return await _db.Instructions
            .Include(i => i.Creator)
            .Include(i => i.Executor)
            .ToArrayAsync();
    }

    private string GetCurrentUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //var user = await _userManager.GetUserAsync(HttpContext.User);
        if (userId is null)
            throw new Exception("User not found");

        return userId;
    }
}