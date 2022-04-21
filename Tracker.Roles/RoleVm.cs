namespace Tracker.Roles;

public class RoleVm
{
    public string Id { get; }
    public string Name { get; }

    public RoleVm(string id, string name)
    {
        Id = id;
        Name = name;
    }
}