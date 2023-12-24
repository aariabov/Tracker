using Microsoft.EntityFrameworkCore;
using Tracker.Instructions.Db;
using Tracker.Instructions.Db.Models;

namespace Tracker.Instructions;

public class UserRepository
{
    private readonly InstructionsDbContext _db;

    public UserRepository(InstructionsDbContext db)
    {
        _db = db;
    }

    public Task<User> GetById(string userId)
    {
        return _db.Users.SingleAsync(u => u.Id == userId);
    }

    public Task<User[]> GetAllUsers()
    {
        return _db.Users.ToArrayAsync();
    }

    public Task<bool> IsUserExistsAsync(string executorId)
    {
        return _db.Users.AnyAsync(u => u.Id == executorId);
    }

    public Task<bool> HasUserChildrenAsync(string userId)
    {
        return _db.Users.AnyAsync(u => u.BossId == userId);
    }

    public void UpdateUser(User user)
    {
        _db.Users.Update(user);
    }

    public void InsertUser(User user)
    {
        _db.Users.Add(user);
    }

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
}
