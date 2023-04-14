using Microsoft.EntityFrameworkCore;
using Tracker.Audit.Db;
using Tracker.Audit.Db.Models;

namespace Tracker.Audit;

public class AuditRepository : IAuditRepository
{
    private readonly AuditDbContext _db;

    public AuditRepository(AuditDbContext db)
    {
        _db = db;
    }

    public async Task<int> CreateLog(AuditLog auditLog)
    {
        _db.AuditLogs.Add(auditLog);
        await _db.SaveChangesAsync();
        return auditLog.Id;
    }

    public async Task DeleteLog(int id)
    {
        var auditLog = await _db.AuditLogs.SingleAsync(l => l.Id == id);
        _db.AuditLogs.Remove(auditLog);
        await _db.SaveChangesAsync();
    }
}
