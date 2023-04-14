using Tracker.Audit.Db.Models;

namespace Tracker.Audit;

public interface IAuditRepository
{
    Task<int> CreateLog(AuditLog auditLog);
    Task DeleteLog(int id);
}
