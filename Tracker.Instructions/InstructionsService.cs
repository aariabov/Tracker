using Tracker.Common;
using Tracker.Db.Models;
using Tracker.Instructions.RequestModels;
using Tracker.Instructions.Validators;
using Tracker.Instructions.ViewModels;
using Tracker.Users;

namespace Tracker.Instructions;

public class InstructionsService : IInstructionsService
{
    private readonly InstructionValidationService _instructionValidationService;
    private readonly IInstructionsRepository _instructionsRepository;
    private readonly UsersService _usersService;
    private readonly IInstructionStatusService _statusService;

    public InstructionsService(InstructionValidationService instructionValidationService
        , IInstructionsRepository instructionsRepository
        , UsersService usersService
        , IInstructionStatusService statusService)
    {
        _instructionValidationService = instructionValidationService;
        _instructionsRepository = instructionsRepository;
        _usersService = usersService;
        _statusService = statusService;
    }

    public async Task<InstructionVm[]> GetUserInstructionsAsync()
    {
        var userId = _usersService.GetCurrentUserId();
        var allInstructions = await _instructionsRepository.GetAllInstructionsAsync();
        var userInstructions = allInstructions
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
        // Функции и процедуры не поддерживаются в SQLite
        // SQL не может содержать связанные данные, но можно потом добавить Include
        // К CTE нельзя добавить экстеншены типа Include, можно только к запросам, которые начинаются с select
        // Можно рекурсивно вытягивать всех детей, но будет много запросов
        // Можно вытянуть все записи (все дети уже будут привязаны), взять родительское поручение,
        // и рекурсивно преобразовать в плоский список - подойдет только для маленьких иерархий
        
        // Итого
        // пока можно просто вытянуть все записи
        // потом, на нормальной базе можно создать функцию и использовать CTE
        // если нужна будет производительность то использовать Closure, Enumeration paths или Nested sets - 
        // во всех случаях будет гемор с пересчетом, но Closure понятнее
        
        var allInstructions = await _instructionsRepository.GetAllInstructionsAsync();
        var instruction = allInstructions.SingleOrDefault(i => i.Id == id);
        if (instruction is null)
            throw new Exception($"User with id {id} not found");
        
        var rootInstruction = GetRoot(instruction);
        var instructions = GetAllChildren(rootInstruction);
        var instructionTreeItemVms = instructions.Select(currentInstruction =>
        {
            var status = _statusService.GetStatus(currentInstruction);
            return InstructionTreeItemVm.Create(currentInstruction, status);
        });
        
        return instructionTreeItemVms.ToArray();
    }
    
    public async Task<Result<int>> CreateInstructionAsync(InstructionRm instructionRm)
    {
        var validationResult = await _instructionValidationService.ValidateInstructionAsync(instructionRm);
        if (!validationResult.IsSuccess)
            return Result.Errors<int>(validationResult.ValidationErrors);

        var userId = _usersService.GetCurrentUserId();
        var newInstruction = new Instruction
        {
            Name = instructionRm.Name,
            CreatorId = userId,
            ExecutorId = instructionRm.ExecutorId,
            ParentId = instructionRm.ParentId,
            Deadline = instructionRm.Deadline
        };

        _instructionsRepository.CreateInstruction(newInstruction);
        await _instructionsRepository.SaveChangesAsync();
        return Result.Ok(newInstruction.Id);
    }
    
    public async Task<Result> SetExecDateAsync(ExecDateRm execDateRm)
    {
        var validationResult = await _instructionValidationService.ValidateExecDateAsync(execDateRm);
        if (!validationResult.IsSuccess)
            return Result.Errors<int>(validationResult.ValidationErrors);
        
        var instruction = await _instructionsRepository.GetInstructionByIdAsync(execDateRm.InstructionId);
        if (instruction is null)
            throw new Exception($"Instruction with id {execDateRm.InstructionId} not found");
        
        instruction.ExecDate = execDateRm.ExecDate;
        _instructionsRepository.UpdateInstruction(instruction);
        await _instructionsRepository.SaveChangesAsync();
        return Result.Ok();
    }
    
    private IEnumerable<Instruction> GetAllChildren(Instruction instruction)
    {
        var stack = new Stack<Instruction>();
        stack.Push(instruction);
        while(stack.Count > 0)
        {
            var current = stack.Pop();
            yield return current;
            foreach(var child in current.Children)
                stack.Push(child);
        }
        
        // реализация рекурсией - понятнее, но медленее и затратнее
        // var result = new List<Instruction> { instruction };
        // foreach (var child in instruction.Children)
        // {
        //     var children = GetAllChildren(child);
        //     result.AddRange(children);
        // }
        //
        // return result;
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
            return false;

        if (instruction.ExecutorId != userId)
            return false;
        
        if (!instruction.Children.Any())
            return true;
        
        return !_statusService.AnyChildInWork(instruction);
    }
}