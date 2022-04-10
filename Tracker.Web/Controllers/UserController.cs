using System.Net;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tracker.Common;
using Tracker.Db.Models;
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
    private readonly UserCreationValidator _userCreationValidator;
    private readonly UserUpdatingValidator _userUpdatingValidator;
    private readonly UserDeletingValidator _userDeletingValidator;
    
    private readonly int _refreshTokenValidityInDays;

    public UserController(UserManager<User> userManager
        , IConfiguration config
        , SignInManager<User> signInManager
        , JwtGenerator jwtGenerator
        , UserCreationValidator userCreationValidator
        , UserUpdatingValidator userUpdatingValidator
        , UserDeletingValidator userDeletingValidator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
        _userCreationValidator = userCreationValidator;
        _userUpdatingValidator = userUpdatingValidator;
        _userDeletingValidator = userDeletingValidator;

        _refreshTokenValidityInDays = config.GetValue<int>("Token:RefreshTokenValidityInDays");
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<IdentityError>))]
    public async Task<ActionResult> RegisterAsync([FromBody]UserRegistrationRm userRm)
    {
        var validationResult = await _userCreationValidator.ValidateAsync(userRm);
        if (!validationResult.IsValid)
            return Ok(validationResult.Errors.Format());
        
        var newUser = new User(userRm.Name, userRm.Email, userRm.BossId);
        var result = await _userManager.CreateAsync(newUser, userRm.Password);
        if (!result.Succeeded) 
            return BadRequest(result.Errors);

        if (userRm.Roles.Any())
        {
            var rolesResult = await _userManager.AddToRolesAsync(newUser, userRm.Roles);
            if (!rolesResult.Succeeded) 
                return BadRequest(result.Errors);
        }
        
        return Ok(newUser.Id);
    }

    [HttpPost("update")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<IdentityError>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> UpdateRole([FromBody]UserUpdatingRm userUpdatingRm)
    {
        var validationResult = await _userUpdatingValidator.ValidateAsync(userUpdatingRm);
        if (!validationResult.IsValid)
            return Ok(validationResult.Errors.Format());

        var updatedUser = await _userManager.FindByIdAsync(userUpdatingRm.Id);
        if (updatedUser is null)
            return NotFound();
        
        updatedUser.UserName = userUpdatingRm.Name;
        updatedUser.Email = userUpdatingRm.Email;
        updatedUser.BossId = userUpdatingRm.BossId;
        
        var result = await _userManager.UpdateAsync(updatedUser);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var userRoles = await _userManager.GetRolesAsync(updatedUser);
        var addedRoles = userUpdatingRm.Roles.Except(userRoles);
        var removedRoles = userRoles.Except(userUpdatingRm.Roles);
 
        var roleAddingResult = await _userManager.AddToRolesAsync(updatedUser, addedRoles);
        if (!roleAddingResult.Succeeded)
            return BadRequest(roleAddingResult.Errors);
        
        var roleRemovingResult = await _userManager.RemoveFromRolesAsync(updatedUser, removedRoles);
        if (!roleRemovingResult.Succeeded)
            return BadRequest(roleRemovingResult.Errors);
        
        return Ok();
    }

    [HttpPost("delete")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModelErrorsVm))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<IdentityError>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> DeleteUser([FromBody]UserDeletingRm userDeletingRm)
    {
        var validationResult = await _userDeletingValidator.ValidateAsync(userDeletingRm);
        if (!validationResult.IsValid)
            return Ok(validationResult.Errors.Format());

        var deletedUser = await _userManager.FindByIdAsync(userDeletingRm.Id);
        if (deletedUser is null)
            return NotFound();
        
        var result = await _userManager.DeleteAsync(deletedUser);
        if (result.Succeeded)
            return Ok();

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

        var token = await CreateToken(user);
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
        
        var token = await CreateToken(user);
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

    private async Task<string> CreateToken(User user)
    {
        var userRolesTask = _userManager.GetRolesAsync(user: user);
        var isUserBossTask = _userManager.Users.AnyAsync(predicate: u => u.BossId == user.Id);
        var userRoles = await userRolesTask;
        var isUserBoss = await isUserBossTask;
        var token = _jwtGenerator.CreateToken(userId: user.Id
            , userEmail: user.Email
            , userRoles: userRoles
            , isUserBoss: isUserBoss);
        return token;
    }
    
    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}