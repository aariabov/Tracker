using Microsoft.AspNetCore.Identity;

namespace Tracker.Db.Models;

public class Role : IdentityRole
{
    public virtual ICollection<User> Users { get; set; }

    public Role(string roleName) : base(roleName)
    {
    }
    
    private Role(){}
}