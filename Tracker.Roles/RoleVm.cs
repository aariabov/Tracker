using Tracker.Db.Models;

namespace Tracker.Roles;

public class RoleVm
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string ConcurrencyStamp { get; set; }

    public RoleVm() { }

    public RoleVm(Role role)
    {
        Id = role.Id;
        Name = role.Name;
        ConcurrencyStamp = role.ConcurrencyStamp;
    }
}