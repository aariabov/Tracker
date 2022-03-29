using System.Net;
using System.Security.Cryptography;
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
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtGenerator _jwtGenerator;
    private readonly UserValidator _userValidator;
    
    private readonly int _refreshTokenValidityInDays;

    public UserController(UserManager<User> userManager
        , SignInManager<User> signInManager
        , JwtGenerator jwtGenerator
        , UserValidator userValidator
        , IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
        _userValidator = userValidator;
        
        _refreshTokenValidityInDays = config.GetValue<int>("Token:RefreshTokenValidityInDays");
    }

    [AllowAnonymous]
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
    
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokensVm))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TokensVm>> LoginAsync(LoginVM loginVm)
    {
        var user = await _userManager.FindByEmailAsync(loginVm.Email);
        if (user is null)
            return NotFound();
        
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginVm.Password, false);
        if (!result.Succeeded) 
            return Unauthorized();
        
        var isUserBoss = await _userManager.Users.AnyAsync(u => u.BossId == user.Id);
        var token = _jwtGenerator.CreateToken(user, isUserBoss);
        var refreshToken = GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_refreshTokenValidityInDays);
        await _userManager.UpdateAsync(user);
        
        return Ok(new TokensVm(token, refreshToken));
    }
    
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokensVm))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokensVm>> RefreshToken([FromBody]string refreshToken)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user is null || user.RefreshTokenExpiryTime < DateTime.Now)
            return Unauthorized();
        
        var isUserBoss = await _userManager.Users.AnyAsync(u => u.BossId == user.Id);
        var token = _jwtGenerator.CreateToken(user, isUserBoss);
        var newRefreshToken = GenerateRefreshToken();
        
        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);
        
        return Ok(new TokensVm(token, newRefreshToken));
    }
    
    [HttpPost("revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Revoke()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user is null)
            return NotFound();
        
        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        return NoContent();
    }
    
    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}