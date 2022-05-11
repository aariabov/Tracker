using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tracker.Db.Models;

namespace Tracker.Roles;

public class RoleRepo : IRoleRepo
{
    private readonly RoleManager<Role> _roleManager;

    public RoleRepo(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<RoleVm[]> GetAllRoles()
    {
        return await _roleManager.Roles
            .OrderBy(r => r.Name)
            .Select(r => new RoleVm(r))
            .ToArrayAsync();
    }

    public async Task<bool> RoleExistsAsync(string role)
    {
        return await _roleManager.RoleExistsAsync(role);
    }

    public async Task<Role?> GetRoleById(string roleId)
    {
        return await _roleManager.Roles.SingleOrDefaultAsync(r => r.Id == roleId);
    }

    public async Task<bool> IsAnyUserBelongToRole(string roleId)
    {
        var query = 
            from role in _roleManager.Roles
            where role.Id == roleId
            select role.Users.Any();
        
        return await query.SingleAsync();
    }
}