using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tracker.Web.Db;
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
    public async Task<IActionResult> RegisterAsync([FromBody]UserRegistrationVm userVm)
    {
        var newUser = new User
        {
            Email = userVm.Email, 
            UserName = userVm.Email
        };
        
        var result = await _userManager.CreateAsync(newUser, userVm.Password);
        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<UserVm>> LoginAsync(LoginVM loginVm)
    {
        var user = await _userManager.FindByEmailAsync(loginVm.Email);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginVm.Password, false);

        if (result.Succeeded)
        {
            var token = _jwtGenerator.CreateToken(user);
            return new UserVm(token, user.Email);
        }

        return Unauthorized();
    }
}