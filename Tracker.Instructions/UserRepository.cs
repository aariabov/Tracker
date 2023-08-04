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

    public Task<User[]> GetAllUsers()
    {
        return _db.Users.ToArrayAsync();
    }
}
