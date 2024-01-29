using FluentValidation;
using Tracker.Instructions.RequestModels;
using User = Tracker.Instructions.Db.Models.User;

namespace Tracker.Instructions.Validators.FluentValidators;

internal sealed class InstructionValidator : AbstractValidator<InstructionRm>
{
    private readonly InstructionsRepository _instructionsRepository;
    private readonly InstructionStatusService _statusService;
    private readonly User _creator;
    private readonly UserRepository _userRepository;

    public InstructionValidator(InstructionsRepository instructionsRepository
        , UserRepository userRepository
        , InstructionStatusService statusService
        , User creator
        , DateTime today)
    {
        _instructionsRepository = instructionsRepository;
        _statusService = statusService;
        _creator = creator;
        _userRepository = userRepository;

        const int nameMinLen = 3;
        const int nameMaxLen = 100;

        RuleFor(instruction => instruction.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Текст поручения не может быть пустым")
            .Length(nameMinLen, nameMaxLen).WithMessage($"Текст поручения должно быть от {nameMinLen} до {nameMaxLen} символов");
        RuleFor(instruction => instruction.ExecutorId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Исполнитель не может быть пустым")
            .MustAsync(ExecutorExistsAsync).WithMessage("Исполнитель не существует");
        RuleFor(instruction => instruction.Deadline)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Дедлайн не может быть пустым")
            .GreaterThanOrEqualTo(today.Date).WithMessage("Дедлайн должен быть больше или равно сегодня");
        RuleFor(instruction => instruction.ParentId)
            .Cascade(CascadeMode.Stop)
            .MustAsync(ParentExistsAsync).WithMessage("Родительское поручение не найдено")
            .MustAsync(BeNotExecutedAsync).WithMessage("Родительское поручение уже исполнено")
            .MustAsync(BeCreatedByMyBossAsync).WithMessage("Родительское поручение должно быть создано Вашим руководителем")
            .When(instruction => instruction.ParentId is not null);
    }

    private async Task<bool> ExecutorExistsAsync(string executorId, CancellationToken token)
    {
        return await _userRepository.IsUserExistsAsync(executorId);
    }

    private async Task<bool> ParentExistsAsync(int? parentId, CancellationToken token)
    {
        return await _instructionsRepository.IsInstructionExistsAsync(parentId!.Value);
    }

    private async Task<bool> BeNotExecutedAsync(int? parentId, CancellationToken token)
    {
        var parentInstruction = await _instructionsRepository.GetInstructionTreeAsync(parentId.Value);
        _statusService.ReCalcStatus(parentInstruction);
        return parentInstruction.StatusId is (int?)ExecStatus.InWork or (int?)ExecStatus.InWorkOverdue;
    }

    private async Task<bool> BeCreatedByMyBossAsync(int? parentId, CancellationToken token)
    {
        var parentInstruction = await _instructionsRepository.GetInstructionByIdAsync(parentId!.Value);
        return parentInstruction!.CreatorId == _creator.BossId &&
               parentInstruction!.ExecutorId == _creator.Id;
    }
}
