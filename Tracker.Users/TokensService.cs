using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Tracker.Common;
using Tracker.Db.Models;
using Tracker.Users.ViewModels;

namespace Tracker.Users;

public class TokensService
{
    private readonly UsersService _userService;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtGenerator _jwtGenerator;
    
    private readonly int _refreshTokenValidityInDays;

    public TokensService(UsersService userService
        , SignInManager<User> signInManager
        , JwtGenerator jwtGenerator
        , IConfiguration config)
    {
        _userService = userService;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
        
        _refreshTokenValidityInDays = Convert.ToInt32(config["Token:RefreshTokenValidityInDays"]);
    }

    public async Task<Result<TokensVm>> LoginAsync(LoginVM loginVm)
    {
        var user = await _userService.GetUserByEmailAsync(loginVm.Email);
        if (user is null)
            return Result.CommonErrors<TokensVm>(new List<string>{"Неверный логин или пароль"});
        
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginVm.Password, false);
        if (!result.Succeeded)
            return Result.CommonErrors<TokensVm>(new List<string>{"Неверный логин или пароль"});

        var token = await CreateToken(user);
        var refreshToken = GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_refreshTokenValidityInDays);
        await _userService.UpdateUserAsync(user);
        
        return Result.Ok(new TokensVm(token, refreshToken));
    }
    
    public async Task<Result<TokensVm>> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userService.GetUserByRefreshTokenAsync(refreshToken);
        if (user is null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            return Result.CommonErrors<TokensVm>(new List<string>{"Неверный refresh token"});
        
        var token = await CreateToken(user);
        var newRefreshToken = GenerateRefreshToken();
        
        user.RefreshToken = newRefreshToken;
        await _userService.UpdateUserAsync(user);
        
        return Result.Ok(new TokensVm(token, newRefreshToken));
    }
    
    public async Task RevokeAsync()
    {
        var user = await _userService.GetCurrentUser();
        if (user is null)
            throw new Exception("User not found");
        
        user.RefreshToken = null;
        await _userService.UpdateUserAsync(user);
    }

    private async Task<string> CreateToken(User user)
    {
        // ошибка при параллельном выполнении запросов
        // https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/#avoiding-dbcontext-threading-issues
        var userRoles = await _userService.GetUserRolesAsync(user);
        var isUserBoss = await _userService.HasUserChildrenAsync(user.Id);
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