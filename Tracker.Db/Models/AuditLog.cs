namespace Tracker.Db.Models;

public class AuditLog
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string EntityId { get; set; }
    public string EntityName { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public DateTime DateTime { get; set; }

    private AuditLog() { }

    public AuditLog(int type, string entityId, string entityName, string userId, DateTime dateTime)
    {
        Type = type;
        EntityId = entityId;
        EntityName = entityName;
        UserId = userId;
        DateTime = dateTime;
    }
}