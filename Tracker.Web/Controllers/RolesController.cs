using System.Net;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Db;
using Tracker.Web.Domain;
using Tracker.Web.RequestModels;
using Tracker.Web.Validators;
using Tracker.Web.ViewModels;

namespace Tracker.Web.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly RoleCreationValidator _roleCreationValidator;
    private readonly RoleUpdatingValidator _roleUpdatingValidator;
    private readonly RoleDeletingValidator _roleDeletingValidator;
    private readonly RoleManager<Role> _roleManager;
    private readonly AppDbContext _db;

    public RolesController(RoleManager<Role> roleManager
        , RoleUpdatingValidator roleUpdatingValidator
        , RoleCreationValidator roleCreationValidator
        , RoleDeletingValidator roleDeletingValidator
        , AppDbContext db)
    {
        _roleManager = roleManager;
        _roleUpdatingValidator = roleUpdatingValidator;
        _roleCreationValidator = roleCreationValidator;
        _roleDeletingValidator = roleDeletingValidator;
        _db = db;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RoleVm>))]
    public async Task<ActionResult<IEnumerable<RoleVm>>> GetAllRoles()
    {
        var roles = await _roleManager.Roles
            .OrderBy(r => r.Name)
            .Select(r => new RoleVm(r.Id, r.Name, !_db.UserRoles.Any(ur => ur.RoleId == r.Id)))
            .ToArrayAsync();
        
        return Ok(roles);
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<IdentityError>))]
    public async Task<ActionResult<string>> CreateRole([FromBody]RoleCreationRm roleCreationRm)
    {
        var validationResult = await _roleCreationValidator.ValidateAsync(roleCreationRm);
        if (!validationResult.IsValid)
            return Ok(validationResult.Errors.Format());

        var newRole = new Role(roleCreationRm.Name);
        var result = await _roleManager.CreateAsync(newRole);
        if (result.Succeeded)
            return Ok(newRole.Id);

        return BadRequest(result.Errors);
    }

    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<IdentityError>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> UpdateRole([FromBody]RoleUpdatingRm roleUpdatingRm)
    {
        var validationResult = await _roleUpdatingValidator.ValidateAsync(roleUpdatingRm);
        if (!validationResult.IsValid)
            return Ok(validationResult.Errors.Format());

        var updatedRole = await _roleManager.FindByIdAsync(roleUpdatingRm.Id);
        if (updatedRole is null)
            return NotFound();
        
        updatedRole.Name = roleUpdatingRm.Name;
        var result = await _roleManager.UpdateAsync(updatedRole);
        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<IdentityError>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> DeleteRole([FromBody]RoleDeletingRm roleDeletingRm)
    {
        var validationResult = await _roleDeletingValidator.ValidateAsync(roleDeletingRm);
        if (!validationResult.IsValid)
            return Ok(validationResult.Errors.Format());

        var deletedRole = await _roleManager.FindByIdAsync(roleDeletingRm.Id);
        if (deletedRole is null)
            return NotFound();
        
        var result = await _roleManager.DeleteAsync(deletedRole);
        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }
}