using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Db;
using Tracker.Web.Domain;
using Tracker.Web.RequestModels;
using Tracker.Web.ViewModels;

namespace Tracker.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize(Roles = "Admin, Analyst")]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AnalyticsController(AppDbContext db)
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
            .Where(i => i.ExecutorId == reportRm.ExecutorId)
            .Where(i => i.Deadline.Date >= reportRm.StartDate.Date)
            .Where(i => i.Deadline.Date <= reportRm.EndDate.Date);
        
        var allStatuses = new ExecStatus[]
        {
            ExecStatus.InWork, 
            ExecStatus.InWorkOverdue, 
            ExecStatus.Completed, 
            ExecStatus.CompletedOverdue
        };
        
        var reportRows = allStatuses.GroupJoin(filteredInstructions
            , s => s, i => i.Status, (s, i) => new EmployeeReportRowVm(s, i.Count()));
        
        return Ok(reportRows);
    }

    [HttpPost("employees-report")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EmployeesReportRowVm>))]
    public async Task<ActionResult<IEnumerable<EmployeesReportRowVm>>> GetEmployeesReport(
        [FromBody] EmployeesReportRm reportRm)
    {
        var allInstructions = await _db.Instructions.Include(i => i.Executor).ToArrayAsync();
        var filteredInstructions = allInstructions
            .Where(i => i.Deadline.Date >= reportRm.StartDate.Date)
            .Where(i => i.Deadline.Date <= reportRm.EndDate.Date);

        var reportRows = filteredInstructions
            .GroupBy(i => i.Executor)
            .Select(g => new EmployeesReportRowVm(
                id: g.Key.Id
                , executor: g.Key.UserName
                , inWorkCount: g.Count(i => i.Status == ExecStatus.InWork)
                , inWorkOverdueCount: g.Count(i => i.Status == ExecStatus.InWorkOverdue)
                , completedCount: g.Count(i => i.Status == ExecStatus.Completed)
                , completedOverdueCount: g.Count(i => i.Status == ExecStatus.CompletedOverdue)))
            .OrderBy(r => r.Executor);
        
        return Ok(reportRows);
    }
}