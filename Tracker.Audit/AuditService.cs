using Tracker.Db.Models;
using Tracker.Db.UnitOfWorks;

namespace Tracker.Audit;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuditService(IAuditRepository auditRepository, IUnitOfWork unitOfWork)
    {
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task LogAndSaveChangesAsync(AuditType type, string entityId, string entityName, string userId)
    {
        LogAsync(type, entityId, entityName, userId);
        await _unitOfWork.SaveChangesAsync();
    }

    public void LogAsync(AuditType type, string entityId, string entityName, string userId)
    {
        var newAuditLog = new AuditLog((int)type, entityId, entityName, userId, DateTime.UtcNow);
        _auditRepository.CreateLog(newAuditLog);
    }
}