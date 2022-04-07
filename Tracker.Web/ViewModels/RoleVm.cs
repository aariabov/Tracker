namespace Tracker.Web.ViewModels;

public class RoleVm
{
    public string Id { get; }
    public string Name { get; }
    public bool CanBeDeleted { get; }

    public RoleVm(string id, string name, bool canBeDeleted)
    {
        Id = id;
        Name = name;
        CanBeDeleted = canBeDeleted;
    }
}