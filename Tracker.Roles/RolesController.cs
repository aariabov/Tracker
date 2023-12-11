using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Riabov.Tracker.Common;
using Tracker.Roles.RequestModels;

namespace Tracker.Roles;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly RolesService _rolesService;

    public RolesController(RolesService rolesService)
    {
        _rolesService = rolesService;
    }

    /// <summary>
    /// Получить список всех ролей
    /// </summary>
    /// <returns></returns>
    [HttpGet("get-all-roles")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RoleVm>))]
    public async Task<ActionResult<IEnumerable<RoleVm>>> GetAllRoles()
    {
        var roles = await _rolesService.GetAllRoles();
        return Ok(roles);
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<IdentityError>))]
    public async Task<ActionResult<string>> CreateRole([FromBody] RoleCreationRm roleCreationRm)
    {
        var result = await _rolesService.CreateRole(roleCreationRm);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Ok(new ModelErrorsVm(result));
    }

    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    public async Task<ActionResult> UpdateRole([FromBody] RoleUpdatingRm roleUpdatingRm)
    {
        var result = await _rolesService.UpdateRole(roleUpdatingRm);
        if (result.IsSuccess)
        {
            return Ok();
        }

        return Ok(new ModelErrorsVm(result));
    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    public async Task<ActionResult> DeleteRole([FromBody] RoleDeletingRm roleDeletingRm)
    {
        var result = await _rolesService.DeleteRole(roleDeletingRm);
        if (result.IsSuccess)
        {
            return Ok();
        }

        return Ok(new ModelErrorsVm(result));
    }
}
