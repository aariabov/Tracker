using Tracker.Db.Models;
using Tracker.Users.ViewModels;

namespace Tracker.Users;

public interface IUserRepository
{
    Task<OrgStructElementVm[]> GetAllUsers();
    Task<User?> GetUserById(string userId);
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserByRefreshToken(string refreshToken);
    Task<bool> HasChildren(string userId);
    Task<bool> IsUserExistsAsync(string userId);
    Task<bool> IsEmailExistsAsync(string email);
    Task<bool> IsRoleExistsAsync(string role);
}