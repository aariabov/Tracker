using Microsoft.AspNetCore.Identity;

namespace Tracker.Db.Models;

public sealed class User : IdentityUser
{
    public string? BossId { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public ICollection<Role> Roles { get; set; }
    public User? Boss { get; set; }
    public ICollection<User>? Children { get; set; }

    public User() { }

    public User(string name, string email, string? bossId)
    {
        UserName = name;
        Email = email;
        BossId = bossId;
    }

    public bool IsAdmin(string adminEmail)
    {
        return Email == adminEmail;
    }
}
