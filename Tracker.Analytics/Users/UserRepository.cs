using Microsoft.EntityFrameworkCore;
using Tracker.Analytics.Db;
using Tracker.Analytics.Db.Models;

namespace Tracker.Analytics.Users;

public class UserRepository
{
    private readonly AnalyticsDbContext _db;

    public UserRepository(AnalyticsDbContext db)
    {
        _db = db;
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
