namespace Tracker.Audit.Models.Params;

public class LogModel
{
    public AuditType Type { get; set; }
    public string EntityId { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
