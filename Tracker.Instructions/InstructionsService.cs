using Riabov.Tracker.Common;
using Tracker.Db.Models;
using Tracker.Instructions.Db.Models;
using Tracker.Instructions.RequestModels;
using Tracker.Instructions.Validators;
using Tracker.Instructions.ViewModels;
using Tracker.Users;
using Instruction = Tracker.Instructions.Db.Models.Instruction;
using User = Tracker.Instructions.Db.Models.User;

namespace Tracker.Instructions;

public class InstructionsService : IInstructionsService
{
    private readonly InstructionValidationService _instructionValidationService;
    private readonly IInstructionsRepository _instructionsRepository;
    private readonly UsersService _usersService;
    private readonly IInstructionStatusService _statusService;
    private readonly TreePathsService _treePathsService;

    public InstructionsService(InstructionValidationService instructionValidationService
        , IInstructionsRepository instructionsRepository
        , UsersService usersService
        , IInstructionStatusService statusService
        , TreePathsService treePathsService)
    {
        _instructionValidationService = instructionValidationService;
        _instructionsRepository = instructionsRepository;
        _usersService = usersService;
        _statusService = statusService;
        _treePathsService = treePathsService;
    }

    public async Task<InstructionVm[]> GetUserInstructionsAsync(int page, int perPage, Sort sort)
    {
        var userId = _usersService.GetCurrentUserId();
        var allUserInstructions = await _instructionsRepository.GetUserInstructionsWithDescendantsAsync(userId, page, perPage, sort);
        var userInstructions = allUserInstructions
            .Where(i => i.CreatorId == userId || i.ExecutorId == userId);

        var isUserBoss = await _usersService.HasUserChildrenAsync(userId);

        var instructionVms = userInstructions.Select(instruction =>
        {
            var canCreateChild = CanUserCreateChild(instruction, userId, isUserBoss);
            var canBeExecuted = CanBeExecuted(instruction, userId);
            var status = _statusService.GetStatus(instruction);
            return InstructionVm.Create(instruction, status, canCreateChild, canBeExecuted);
        });

        return instructionVms.ToArray();
    }

    public async Task<InstructionTreeItemVm[]> GetTreeInstructionAsync(int id)
    {
        // SQL не может содержать связанные данные, но можно потом добавить Include
        // К CTE нельзя добавить экстеншены типа Include, можно только к запросам, которые начинаются с select
        // Можно рекурсивно вытягивать всех детей, но будет много запросов
        // Можно вытянуть все записи (все дети уже будут привязаны), взять родительское поручение,
        // и рекурсивно преобразовать в плоский список - подойдет только для маленьких иерархий

        // сейчас иерархия загружается с помощью CTE
        // в планах добавить Closure, Enumeration paths и Nested sets

        var instruction = await _instructionsRepository.GetInstructionTreeAsync(id);
        if (instruction is null)
        {
            throw new Exception($"Instruction with id {id} not found");
        }

        var rootInstruction = GetRoot(instruction);
        var instructions = Helpers.GetAllChildren(rootInstruction);
        var instructionTreeItemVms = instructions.Select(currentInstruction =>
        {
            var status = _statusService.GetStatus(currentInstruction);
            return InstructionTreeItemVm.Create(currentInstruction, status);
        });

        return instructionTreeItemVms.ToArray();
    }

    // TODO: можно написать юнит тест
    public async Task<Result<int>> CreateInstructionAsync(InstructionRm instructionRm, User creator, DateTime today)
    {
        var validationResult = await _instructionValidationService.ValidateInstructionAsync(instructionRm, creator, today);
        if (!validationResult.IsSuccess)
        {
            return Result.Errors<int>(validationResult.ValidationErrors);
        }

        var newInstruction = new Instruction
        {
            Name = instructionRm.Name,
            CreatorId = creator.Id,
            ExecutorId = instructionRm.ExecutorId,
            ParentId = instructionRm.ParentId,
            Deadline = instructionRm.Deadline.Date
        };

        _instructionsRepository.CreateInstruction(newInstruction);
        await _instructionsRepository.SaveChangesAsync();

        await _treePathsService.UpdateInstructionTreePath(newInstruction);
        await UpdateInstructionClosure(newInstruction.Id, newInstruction.ParentId);
        await _instructionsRepository.SaveChangesAsync();
        return Result.Ok(newInstruction.Id);
    }

    private async Task UpdateInstructionClosure(int id, int? parentId)
    {
        await _instructionsRepository.UpdateInstructionClosureAsync(id, parentId);
    }

    public async Task<Result> SetExecDateAsync(ExecDateRm execDateRm, string executorId, DateTime today)
    {
        var validationResult = await _instructionValidationService.ValidateExecDateAsync(execDateRm, executorId, today);
        if (!validationResult.IsSuccess)
        {
            return Result.Errors<int>(validationResult.ValidationErrors);
        }

        var instruction = await _instructionsRepository.GetInstructionByIdAsync(execDateRm.InstructionId);
        if (instruction is null)
        {
            throw new Exception($"Instruction with id {execDateRm.InstructionId} not found");
        }

        instruction.ExecDate = execDateRm.ExecDate.Date;
        _instructionsRepository.UpdateInstruction(instruction);
        await _instructionsRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task RecalculateAllClosureTable()
    {
        await _instructionsRepository.RecalculateAllInstructionsClosuresAsync();
    }

    public async Task<int> GetTotalUserInstructionsAsync()
    {
        var userId = _usersService.GetCurrentUserId();
        var total = await _instructionsRepository.GetTotalUserInstructionsAsync(userId);
        return total;
    }

    private Instruction GetRoot(Instruction instruction)
    {
        while (instruction.Parent is not null)
        {
            instruction = instruction.Parent;
        }

        return instruction;
    }

    private bool CanUserCreateChild(Instruction instruction, string userId, bool isUserBoss)
    {
        return instruction.ExecutorId == userId && isUserBoss && instruction.ExecDate is null;
    }

    private bool CanBeExecuted(Instruction instruction, string userId)
    {
        if (instruction.ExecDate is not null)
        {
            return false;
        }

        if (instruction.ExecutorId != userId)
        {
            return false;
        }

        if (!instruction.Children.Any())
        {
            return true;
        }

        return !_statusService.AnyChildInWork(instruction);
    }
}
