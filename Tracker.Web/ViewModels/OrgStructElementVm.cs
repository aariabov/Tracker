namespace Tracker.Web.ViewModels;

public class OrgStructElementVm
{
    public string Id { get; }
    public string Name { get; }
    public string? ParentId { get; }

    public OrgStructElementVm(string id, string name, string? parentId)
    {
        Id = id;
        Name = name;
        ParentId = parentId;
    }
}