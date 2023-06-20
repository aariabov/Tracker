namespace Tracker.Analytics.Db.Models;

public class Instruction
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CreatorId { get; set; }
    public string ExecutorId { get; set; }
    public User? Creator { get; set; }
    public User? Executor { get; set; }
    public ExecStatus StatusId { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime? ExecDate { get; set; }
}
