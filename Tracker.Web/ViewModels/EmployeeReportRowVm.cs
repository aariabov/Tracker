using Tracker.Web.Domain;

namespace Tracker.Web.ViewModels;

public class EmployeeReportRowVm
{
    public ExecStatus Id { get; }
    public string Status { get; }
    public int Count { get; }

    public EmployeeReportRowVm(ExecStatus status, int count)
    {
        Id = status;
        Status = status.GetString();
        Count = count;
    }
}