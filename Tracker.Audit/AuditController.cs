using Microsoft.AspNetCore.Mvc;
using Tracker.Audit.Models.Params;

namespace Tracker.Audit;

[ApiController]
[Route("api/audit")]
public class AuditController : ControllerBase
{
    private readonly AuditService _auditService;

    public AuditController(AuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpPost("create-log")]
    public Task<int> CreateLog(LogModel model)
    {
        return _auditService.LogAsync(model.Type, model.EntityId, model.EntityName, model.UserId);
    }

    [HttpPost("delete-log")]
    public Task DeleteLog(DeleteLogModel model)
    {
        return _auditService.DeleteLog(model.Id);
    }
}
