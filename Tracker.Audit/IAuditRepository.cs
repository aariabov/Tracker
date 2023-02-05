using Tracker.Db.Models;

namespace Tracker.Audit;

public interface IAuditRepository
{
    void CreateLog(AuditLog auditLog);
}
