namespace Tracker.Instructions.RequestModels;

public class InstructionRm
{
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string ExecutorId { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
}