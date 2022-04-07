using Microsoft.AspNetCore.Identity;

namespace Tracker.Web.Domain;

public class Role : IdentityRole
{
    public virtual ICollection<User> Users { get; set; }

    public Role(string roleName) : base(roleName)
    {
    }
    
    private Role(){}
}