using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tracker.Db;
using Tracker.Db.Models;
using Tracker.Users.ViewModels;

namespace Tracker.Users;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _db;

    public UserRepository(UserManager<User> userManager, AppDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<OrgStructElementVm[]> GetAllUsers()
    {
        var query = from user in _userManager.Users
            orderby user.UserName
            select new OrgStructElementVm
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                ParentId = user.BossId,
                Roles = user.Roles.Select(role => role.Name)
            };
        
        var allUsers = await query.ToArrayAsync();
        return allUsers;
    }

    public async Task<User?> GetUserById(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<User?> GetUserByRefreshToken(string refreshToken)
    {
        return await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }

    public async Task<bool> HasChildren(string userId)
    {
        return await _userManager.Users.AnyAsync(u => u.BossId == userId);
    }

    public async Task<bool> IsUserExistsAsync(string userId)
    {
        return await _userManager.Users.AnyAsync(u => u.Id == userId);
    }

    public async Task<bool> IsEmailExistsAsync(string email)
    {
        return await _userManager.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> IsRoleExistsAsync(string role)
    {
        return await _db.Roles.AnyAsync(r => r.Name == role);
    }
}