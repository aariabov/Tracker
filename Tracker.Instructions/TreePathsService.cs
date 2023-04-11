using Tracker.Common.Progress;
using Tracker.Db.Models;
using Tracker.Instructions.Common.Progress;

namespace Tracker.Instructions;

public class TreePathsService
{
    public const char TreePathDelimiter = '/';

    private readonly IInstructionsRepository _instructionsRepository;
    private readonly Progress _progress;

    public TreePathsService(IInstructionsRepository instructionsRepository, Progress progress)
    {
        _instructionsRepository = instructionsRepository;
        _progress = progress;
    }

    // TODO: можно написать юнит тест
    public async Task RunJob(ClientSocketRm socket, int taskId)
    {
        await _instructionsRepository.UpdateAllTreePathsToNullAsync();

        var total = await _instructionsRepository.GetRootInstructionCountAsync();
        var rootInstructionIds = await _instructionsRepository.GetRootInstructionIdsAsync();
        var i = 1;
        foreach (var rootInstructionId in rootInstructionIds)
        {
            // для персчета обязательно используем GetInstructionTreeByCteAsync,
            // тк он работает с id/parentId, а они всегда актуальные
            var instructionTree = await _instructionsRepository.GetInstructionTreeByCteAsync(rootInstructionId);
            UpdateTreePaths(instructionTree);
            _instructionsRepository.UpdateInstruction(instructionTree);
            await _instructionsRepository.SaveChangesAsync();
            _progress.NotifyClient(i, total, socket, frequency: 5, taskId);
            i++;
        }

        void UpdateTreePaths(Instruction rootInstruction)
        {
            var instructions = Helpers.GetAllChildren(rootInstruction);
            foreach (var currentInstruction in instructions)
            {
                currentInstruction.TreePath = currentInstruction.Parent is null
                    ? currentInstruction.Id.ToString()
                    : $"{currentInstruction.Parent.TreePath}{TreePathDelimiter}{currentInstruction.Id}";
            }
        }
    }

    public async Task UpdateInstructionTreePath(Instruction instruction)
    {
        var treePath = instruction.Id.ToString();
        if (instruction.ParentId.HasValue)
        {
            var parentInstruction = await _instructionsRepository.GetInstructionByIdAsync(instruction.ParentId.Value);
            treePath = $"{parentInstruction.TreePath}{TreePathDelimiter}{instruction.Id}";
        }
        instruction.TreePath = treePath;
    }
}
