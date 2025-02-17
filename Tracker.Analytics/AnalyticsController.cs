using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tracker.Analytics.Db;
using Tracker.Analytics.RequestModels;
using Tracker.Analytics.ViewModels;

namespace Tracker.Analytics;

// TODO: вынести в репозиторий
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly AnalyticsDbContext _db;

    public AnalyticsController(AnalyticsDbContext db)
    {
        _db = db;
    }

    [HttpPost("employee-report")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EmployeeReportRowVm>))]
    public async Task<ActionResult<IEnumerable<EmployeeReportRowVm>>> GetEmployeeReport(
        [FromBody] EmployeeReportRm reportRm)
    {
        var allInstructions = await _db.Instructions.ToArrayAsync();
        var filteredInstructions = allInstructions
            .Where(i =>
                i.ExecutorId == reportRm.ExecutorId &&
                i.Deadline.Date >= reportRm.StartDate.Date &&
                i.Deadline.Date <= reportRm.EndDate.Date);

        var allStatuses = new ExecStatus[]
        {
            ExecStatus.InWork,
            ExecStatus.InWorkOverdue,
            ExecStatus.Completed,
            ExecStatus.CompletedOverdue
        };

        var reportRows =
            from status in allStatuses
            join instruction in filteredInstructions
                on status equals instruction.StatusId
                into instructions
            select new EmployeeReportRowVm(status, instructions.Count());

        return Ok(reportRows.ToArray());
    }

    [HttpPost("employees-report")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EmployeesReportRowVm>))]
    public async Task<ActionResult<IEnumerable<EmployeesReportRowVm>>> GetEmployeesReport(
        [FromBody] EmployeesReportRm reportRm)
    {
        var allInstructions = await _db.Instructions.Include(i => i.Executor).ToArrayAsync();
        var filteredInstructions = allInstructions
            .Where(i =>
                i.Deadline.Date >= reportRm.StartDate.Date &&
                i.Deadline.Date <= reportRm.EndDate.Date);

        var statusInfos = filteredInstructions.Select(i => new
        {
            Executor = i.Executor,
            Status = i.StatusId
        });

        var reportRows = from statusInfo in statusInfos
                         group statusInfo by statusInfo.Executor into g
                         orderby g.Key.UserName
                         select new EmployeesReportRowVm(
                             id: g.Key.Id,
                             executor: g.Key.UserName,
                             inWorkCount: g.Count(i => i.Status == ExecStatus.InWork),
                             inWorkOverdueCount: g.Count(i => i.Status == ExecStatus.InWorkOverdue),
                             completedCount: g.Count(i => i.Status == ExecStatus.Completed),
                             completedOverdueCount: g.Count(i => i.Status == ExecStatus.CompletedOverdue));

        return Ok(reportRows.ToArray());
    }
}
