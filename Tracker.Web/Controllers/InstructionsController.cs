using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Db;
using Tracker.Web.Domain;
using Tracker.Web.RequestModels;
using Tracker.Web.Validators;
using Tracker.Web.ViewModels;

namespace Tracker.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstructionsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;
    private readonly InstructionValidator _instructionValidator;
    private readonly ExecDateValidator _execDateValidator;
    
    public InstructionsController(AppDbContext db
        , UserManager<User> userManager
        , InstructionValidator instructionValidator
        , ExecDateValidator execDateValidator)
    {
        _db = db;
        _userManager = userManager;
        _instructionValidator = instructionValidator;
        _execDateValidator = execDateValidator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<InstructionVm>))]
    public async Task<ActionResult<IEnumerable<InstructionVm>>> GetUserInstructions()
    {
        var userId = GetCurrentUserId();
        var allInstructions = await GetAllInstructions();
        var userInstructions = allInstructions
            .Where(i => i.CreatorId == userId || i.ExecutorId == userId);
        
        // TODO: надо внедрять информацию об юзере через DI
        var isUserBoss = User.FindFirstValue("isUserBoss") == "true";

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

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    public async Task<ActionResult<int>> CreateInstruction([FromBody]InstructionRm instructionRm)
    {
        var validationResult = await _instructionValidator.ValidateAsync(instructionRm);
        if (!validationResult.IsValid)
            return Ok(validationResult.Errors.Format());
        
        var userId = GetCurrentUserId();
        var newInstruction = new Instruction
        {
            Name = instructionRm.Name,
            CreatorId = userId,
            ExecutorId = instructionRm.ExecutorId,
            ParentId = instructionRm.ParentId,
            Deadline = instructionRm.Deadline
        };
        
        _db.Instructions.Add(newInstruction);
        await _db.SaveChangesAsync();
        return Ok(newInstruction.Id);
    }
    
    [HttpPost("set-exec-date")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetExecDate([FromBody] ExecDateRm execDateRm)
    {
        var validationResult = await _execDateValidator.ValidateAsync(execDateRm);
        if (!validationResult.IsValid)
            return Ok(validationResult.Errors.Format());
        
        var userId = GetCurrentUserId();
        var instruction = _db.Instructions.Single(i => i.Id == execDateRm.InstructionId);
        instruction.SetExecDate(execDateRm.ExecDate, userId);
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