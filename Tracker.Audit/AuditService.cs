using Tracker.Audit.Db.Models;

namespace Tracker.Audit;

public class AuditService
{
    private readonly AuditRepository _auditRepository;

    public AuditService(AuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public Task<int> LogAsync(AuditType type, string entityId, string entityName, string userId)
    {
        var newAuditLog = new AuditLog((int)type, entityId, entityName, userId, DateTime.UtcNow);
        return _auditRepository.CreateLog(newAuditLog);
    }

    public Task DeleteLog(int id)
    {
        return _auditRepository.DeleteLog(id);
    }
}
