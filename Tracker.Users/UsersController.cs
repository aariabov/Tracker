using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tracker.Common;
using Tracker.Users.RequestModels;
using Tracker.Users.ViewModels;

namespace Tracker.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;
    private readonly TokensService _tokensService;

    public UsersController(UsersService usersService, TokensService tokensService)
    {
        _usersService = usersService;
        _tokensService = tokensService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrgStructElementVm>))]
    public async Task<ActionResult<IEnumerable<OrgStructElementVm>>> GetAllUsers()
    {
        var allUsers = await _usersService.GetAllUsersAsync();
        return Ok(allUsers);
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    public async Task<ActionResult> RegisterAsync([FromBody] UserRegistrationRm userRm)
    {
        var result = await _usersService.RegisterAsync(userRm);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Ok(new ModelErrorsVm(result));
    }

    [HttpPost("update")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    public async Task<ActionResult> UpdateUserAsync([FromBody] UserUpdatingRm userUpdatingRm)
    {
        var result = await _usersService.UpdateUserAsync(userUpdatingRm);
        if (result.IsSuccess)
        {
            return Ok();
        }

        return Ok(new ModelErrorsVm(result));
    }

    [HttpPost("delete")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    public async Task<ActionResult<string>> DeleteUserAsync([FromBody] UserDeletingRm userDeletingRm)
    {
        var result = await _usersService.DeleteUserAsync(userDeletingRm);
        if (result.IsSuccess)
        {
            return Ok();
        }

        return Ok(new ModelErrorsVm(result));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokensVm))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokensVm>> LoginAsync(LoginVM loginVm)
    {
        var result = await _tokensService.LoginAsync(loginVm);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Unauthorized(new ModelErrorsVm(result));
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokensVm))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokensVm>> RefreshToken([FromBody] string refreshToken)
    {
        var result = await _tokensService.RefreshTokenAsync(refreshToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Unauthorized(new ModelErrorsVm(result));
    }

    [HttpPost("revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Revoke()
    {
        await _tokensService.RevokeAsync();
        return NoContent();
    }
}
