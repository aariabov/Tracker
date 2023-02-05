namespace Tracker.Audit;

public interface IAuditService
{
    void LogAsync(AuditType type, string entityId, string entityName, string userId);
    Task LogAndSaveChangesAsync(AuditType type, string entityId, string entityName, string userId);
}
