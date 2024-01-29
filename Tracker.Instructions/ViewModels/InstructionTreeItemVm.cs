using Tracker.Instructions.Db.Models;

namespace Tracker.Instructions.ViewModels;

public class InstructionTreeItemVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string CreatorName { get; set; }
    public string ExecutorName { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime? ExecDate { get; set; }
    public string Status { get; set; }

    public static InstructionTreeItemVm Create(Instruction instruction)
    {
        return new InstructionTreeItemVm
        {
            Id = instruction.Id
            ,
            Name = instruction.Name
            ,
            ParentId = instruction.ParentId
            ,
            CreatorName = instruction.Creator.UserName
            ,
            ExecutorName = instruction.Executor.UserName
            ,
            Deadline = instruction.Deadline
            ,
            ExecDate = instruction.ExecDate
            ,
            Status = ((ExecStatus)instruction.StatusId).GetString()
        };
    }
}
