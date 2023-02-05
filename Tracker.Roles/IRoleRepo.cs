using Tracker.Db.Models;

namespace Tracker.Roles;

public interface IRoleRepo
{
    Task<RoleVm[]> GetAllRoles();
    Task<bool> RoleExistsAsync(string role);
    Task<Role?> GetRoleById(string roleId);
    Task<bool> IsAnyUserBelongToRole(string roleId);
}
