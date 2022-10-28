using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Tracker.Common;
using Tracker.Common.Progress;
using Tracker.Instructions.RequestModels;
using Tracker.Instructions.ViewModels;
using Tracker.Users;

namespace Tracker.Instructions;

[ApiController]
[Route("api/[controller]")]
public class InstructionsController : ControllerBase
{
    private readonly IInstructionsService _instructionsService;
    private readonly InstructionGeneratorService _instructionGeneratorService;
    private readonly UsersService _usersService;
    private readonly Progress _progress;
    private readonly TreePathsService _treePathsService;
    
    public InstructionsController(IInstructionsService instructionsService,
        InstructionGeneratorService instructionGeneratorService,
        UsersService usersService,
        Progress progress,
        TreePathsService treePathsService)
    {
        _instructionsService = instructionsService;
        _instructionGeneratorService = instructionGeneratorService;
        _usersService = usersService;
        _progress = progress;
        _treePathsService = treePathsService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<InstructionVm>))]
    public async Task<ActionResult<IEnumerable<InstructionVm>>> GetUserInstructions(int page, int perPage, string sort = "name")
    {
        var sortEnum = Helpers.GetSort(sort);
        var instructionVms = await _instructionsService.GetUserInstructionsAsync(page, perPage, sortEnum);
        return Ok(instructionVms);
    }

    [HttpGet("total")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<ActionResult<IEnumerable<InstructionVm>>> GetTotalUserInstructions()
    {
        var total = await _instructionsService.GetTotalUserInstructionsAsync();
        return Ok(total);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<InstructionTreeItemVm>))]
    public async Task<ActionResult<IEnumerable<InstructionTreeItemVm>>> GetTreeInstruction(int id)
    {
        var instructionTreeItemVms = await _instructionsService.GetTreeInstructionAsync(id);
        return Ok(instructionTreeItemVms);
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    public async Task<ActionResult<int>> CreateInstruction([FromBody]InstructionRm instructionRm)
    {
        var user = await _usersService.GetCurrentUser();
        var result = await _instructionsService.CreateInstructionAsync(instructionRm, user, DateTime.UtcNow);
        if (result.IsSuccess)
            return Ok(result.Value);

        return Ok(new ModelErrorsVm(result));
    }
    
    [HttpPost("set-exec-date")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    public async Task<ActionResult> SetExecDate([FromBody] ExecDateRm execDateRm)
    {
        var userId = _usersService.GetCurrentUserId();
        var result = await _instructionsService.SetExecDateAsync(execDateRm, userId, DateTime.UtcNow);
        if (result.IsSuccess)
            return Ok();

        return Ok(new ModelErrorsVm(result));
    }
    
    [HttpPost("recalculate-all-tree-paths")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> RecalculateAllTreePaths(ProgressRm model)
    {
        await _treePathsService.RunJob(model.SocketInfo, model.TaskId);
        return Ok();
    }
    
    [HttpPost("recalculate-all-closure-table")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> RecalculateAllClosureTable()
    {
        await _instructionsService.RecalculateAllClosureTable();
        return Ok();
    }
    
    [HttpPost("generate-instructions")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GenerateInstructions(ProgressRm<GenerationRm> model)
    {
        await _instructionGeneratorService.RunJob(model.SocketInfo, model.Pars, model.TaskId);
        return Ok();
    }
}