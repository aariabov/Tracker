using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Domain;
using Tracker.Web.RequestModels;
using Tracker.Web.Validators;
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
    private readonly UserValidator _userValidator;

    public UserController(UserManager<User> userManager
        , SignInManager<User> signInManager
        , JwtGenerator jwtGenerator
        , UserValidator userValidator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
        _userValidator = userValidator;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<IdentityError>))]
    public async Task<ActionResult> RegisterAsync([FromBody]UserRegistrationRm userRm)
    {
        var validationResult = await _userValidator.ValidateAsync(userRm);
        if (!validationResult.IsValid)
            return Ok(validationResult.Errors.Format());
        
        var newUser = new User(userRm.Name, userRm.Email, userRm.BossId);
        var result = await _userManager.CreateAsync(newUser, userRm.Password);
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