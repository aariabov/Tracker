using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tracker.Common;
using Tracker.Instructions.RequestModels;
using Tracker.Instructions.ViewModels;

namespace Tracker.Instructions;

[ApiController]
[Route("api/[controller]")]
public class InstructionsController : ControllerBase
{
    private readonly IInstructionsService _instructionsService;
    
    public InstructionsController(IInstructionsService instructionsService)
    {
        _instructionsService = instructionsService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<InstructionVm>))]
    public async Task<ActionResult<IEnumerable<InstructionVm>>> GetUserInstructions()
    {
        var instructionVms = await _instructionsService.GetUserInstructionsAsync();
        return Ok(instructionVms);
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
        var result = await _instructionsService.CreateInstructionAsync(instructionRm);
        if (result.IsSuccess)
            return Ok(result.Value);

        return Ok(new ModelErrorsVm(result));
    }
    
    [HttpPost("set-exec-date")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    public async Task<ActionResult> SetExecDate([FromBody] ExecDateRm execDateRm)
    {
        var result = await _instructionsService.SetExecDateAsync(execDateRm);
        if (result.IsSuccess)
            return Ok();

        return Ok(new ModelErrorsVm(result));
    }
    
    [HttpPost("recalculate-all-tree-paths")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> RecalculateAllTreePaths()
    {
        await _instructionsService.RecalculateAllTreePaths();
        return Ok();
    }
    
    [HttpPost("recalculate-all-closure-table")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> RecalculateAllClosureTable()
    {
        await _instructionsService.RecalculateAllClosureTable();
        return Ok();
    }
}