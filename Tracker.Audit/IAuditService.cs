namespace Tracker.Audit;

public interface IAuditService
{
    Task<int> LogAsync(AuditType type, string entityId, string entityName, string userId);
    Task DeleteLog(int id);
}
