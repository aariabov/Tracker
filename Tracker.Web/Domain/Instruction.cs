namespace Tracker.Web.Domain;

public class Instruction
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int CreatorId { get; set; }
    public int ExecutorId { get; set; }
    public OrgStructElement? Creator { get; set; }
    public OrgStructElement? Executor { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime? ExecDate { get; set; }
    public ICollection<Instruction> Children { get; set; } = new List<Instruction>();

    
    public ExecStatus Status
    {
        get
        {
            if (!Children.Any())
                return GetStatusDespiteChildren;

            var inWorkStatuses = new List<ExecStatus> { ExecStatus.InWork, ExecStatus.InWorkOverdue };
            var anyChildInWork = Children.Any(c => inWorkStatuses.Contains(c.Status));
        
            return anyChildInWork ? GetInWorkStatus : GetStatusDespiteChildren;
        }
    }

    private ExecStatus GetStatusDespiteChildren => ExecDate is null ? GetInWorkStatus : GetCompletedStatus;
    private ExecStatus GetInWorkStatus => DateTime.Today > Deadline ? ExecStatus.InWorkOverdue : ExecStatus.InWork;
    private ExecStatus GetCompletedStatus => ExecDate > Deadline ? ExecStatus.CompletedOverdue : ExecStatus.Completed;
}