using Tracker.Web.Domain;

namespace Tracker.Web.ViewModels;

public class EmployeesReportRowVm
{
    public string Id { get; }
    public string Executor { get; }
    public int InWorkCount { get; }
    public int InWorkOverdueCount { get; }
    public int CompletedCount { get; }
    public int CompletedOverdueCount { get; }

    public EmployeesReportRowVm(string id
        , string executor
        , int inWorkCount
        , int inWorkOverdueCount
        , int completedCount
        , int completedOverdueCount)
    {
        Id = id;
        Executor = executor;
        InWorkCount = inWorkCount;
        InWorkOverdueCount = inWorkOverdueCount;
        CompletedCount = completedCount;
        CompletedOverdueCount = completedOverdueCount;
    }
}