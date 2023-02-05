using Tracker.Db;
using Tracker.Db.Models;

namespace Tracker.Audit;

public class AuditRepository : IAuditRepository
{
    private readonly AppDbContext _db;

    public AuditRepository(AppDbContext db)
    {
        _db = db;
    }

    public void CreateLog(AuditLog auditLog)
    {
        _db.AuditLogs.Add(auditLog);
    }
}
