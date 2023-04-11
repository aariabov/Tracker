using Tracker.Db.Models;
using Tracker.Instructions.Common;
using Tracker.Instructions.RequestModels;
using Tracker.Instructions.ViewModels;

namespace Tracker.Instructions;

public interface IInstructionsService
{
    Task<InstructionVm[]> GetUserInstructionsAsync(int page, int perPage, Sort sort);
    Task<InstructionTreeItemVm[]> GetTreeInstructionAsync(int id);
    Task<Result<int>> CreateInstructionAsync(InstructionRm instructionRm, User creator, DateTime today);
    Task<Result> SetExecDateAsync(ExecDateRm execDateRm, string executorId, DateTime today);
    Task RecalculateAllClosureTable();
    Task<int> GetTotalUserInstructionsAsync();
}
