using Tracker.Common;
using Tracker.Instructions.RequestModels;
using Tracker.Instructions.ViewModels;

namespace Tracker.Instructions;

public interface IInstructionsService
{
    Task<InstructionVm[]> GetUserInstructionsAsync();
    Task<InstructionTreeItemVm[]> GetTreeInstructionAsync(int id);
    Task<Result<int>> CreateInstructionAsync(InstructionRm instructionRm);
    Task<Result> SetExecDateAsync(ExecDateRm execDateRm);
    Task RecalculateAllTreePaths();
    Task RecalculateAllClosureTable();
}