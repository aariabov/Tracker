using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Tracker.Db.Models;

namespace Tracker.Users;

public interface IUserManagerService
{
    Task<IdentityResult> CreateAsync(User user, string password);
    Task<IdentityResult> AddToRolesAsync(User user, IEnumerable<string> roles);
    Task<IList<string>> GetRolesAsync(User user);
    Task<User> FindByIdAsync(string userId);
    Task<IdentityResult> RemoveFromRolesAsync(User user, IEnumerable<string> roles);
    Task<IdentityResult> UpdateAsync(User user);
    Task<IdentityResult> DeleteAsync(User user);
    Task<User> GetUserAsync(ClaimsPrincipal principal);
}
