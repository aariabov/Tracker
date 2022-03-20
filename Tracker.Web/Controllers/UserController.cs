using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IdentityResult> RegisterAsync([FromBody]UserRegistrationVm userVm)
    {
        var newUser = new User(userVm.Name, userVm.Email, userVm.BossId);
        var result = await _userManager.CreateAsync(newUser, userVm.Password);
        return result;
    }
    
    [HttpPost("login")]
    public async Task<string> LoginAsync(LoginVM loginVm)
    {
        var user = await _userManager.FindByEmailAsync(loginVm.Email);
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginVm.Password, false);

        if (result.Succeeded)
            return _jwtGenerator.CreateToken(user);

        throw new Exception();
    }
}