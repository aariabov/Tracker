using Tracker.Common;
using Tracker.Db.Models;

namespace Tracker.Instructions.ViewModels;

public class InstructionVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string CreatorName { get; set; }
    public string ExecutorName { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime? ExecDate { get; set; }
    public string Status { get; set; }
    public bool CanCreateChild { get; set; }
    public bool CanBeExecuted { get; set; }

    public static InstructionVm Create(Instruction instruction, ExecStatus status
        , bool canCreateChild, bool canBeExecuted)
    {
        return new InstructionVm
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
            Status = status.GetString()
            ,
            CanCreateChild = canCreateChild
            ,
            CanBeExecuted = canBeExecuted
        };
    }
}
