namespace Tracker.Db.Models;

// TODO: удалить, тк не используется
public class OrgStructElement
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentId { get; set; }

    public OrgStructElement(string name, int? parentId)
    {
        Name = name;
        ParentId = parentId;
    }
}