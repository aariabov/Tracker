using Microsoft.AspNetCore.Identity;

namespace Tracker.Web.Domain;

public sealed class User : IdentityUser
{
    public string? BossId { get; set; }

    private User(){}

    public User(string name, string email, string? bossId)
    {
        UserName = name;
        Email = email;
        BossId = bossId;
    }
}