namespace Tracker.Instructions.Db.Models;

public class InstructionClosure
{
    public int ParentId { get; set; }
    public int Id { get; set; }
    public int Depth { get; set; }
}
