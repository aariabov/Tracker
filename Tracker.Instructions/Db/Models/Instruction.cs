namespace Tracker.Instructions.Db.Models;

public class Instruction
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string? TreePath { get; set; }
    public string CreatorId { get; set; }
    public string ExecutorId { get; set; }
    public User? Creator { get; set; }
    public User? Executor { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime? ExecDate { get; set; }
    public Instruction? Parent { get; set; }
    public int? StatusId { get; set; }
    public ICollection<Instruction> Children { get; set; } = new List<Instruction>();

    public override string ToString()
    {
        return $"{Id} {Name}";
    }
}
