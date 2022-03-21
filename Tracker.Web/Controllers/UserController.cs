using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Domain;
using Tracker.Web.ViewModels;

namespace Tracker.Web.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtGenerator _jwtGenerator;

    public UserController(UserManager<User> userManager, SignInManager<User> signInManager, JwtGenerator jwtGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<IdentityError>))]
    public async Task<ActionResult> RegisterAsync([FromBody]UserRegistrationVm userVm)
    {
        // TODO: валидация
        var newUser = new User(userVm.Name, userVm.Email, userVm.BossId);
        var result = await _userManager.CreateAsync(newUser, userVm.Password);
        if (result.Succeeded) 
            return Ok(newUser.Id);

        return BadRequest(result.Errors);
    }
    
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> LoginAsync(LoginVM loginVm)
    {
        var user = await _userManager.FindByEmailAsync(loginVm.Email);
        if (user is null)
            return NotFound();
        
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginVm.Password, false);
        if (!result.Succeeded) 
            return Unauthorized();
        
        var isUserBoss = await _userManager.Users.AnyAsync(u => u.BossId == user.Id);
        return Ok(_jwtGenerator.CreateToken(user, isUserBoss));
    }
}