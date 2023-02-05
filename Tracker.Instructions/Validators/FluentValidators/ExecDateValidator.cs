using FluentValidation;
using Tracker.Instructions.RequestModels;

namespace Tracker.Instructions.Validators.FluentValidators;

internal class ExecDateValidator : AbstractValidator<ExecDateRm>
{
    private readonly IInstructionsRepository _instructionsRepository;
    private readonly IInstructionStatusService _statusService;
    private readonly string _executorId;

    public ExecDateValidator(IInstructionsRepository instructionsRepository
        , IInstructionStatusService statusService
        , string executorId
        , DateTime today)
    {
        _instructionsRepository = instructionsRepository;
        _statusService = statusService;
        _executorId = executorId;

        RuleFor(rm => rm)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Идентификатор поручения не может быть пустым")
            .CustomAsync(MustBeValidInstruction);
        RuleFor(rm => rm.ExecDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Дата исполнения не может быть пустой")
            .Must(execDate => execDate.Date == today.Date).WithMessage("Дата исполнения должна быть сегодня");
    }

    private async Task MustBeValidInstruction(ExecDateRm execDateRm, ValidationContext<ExecDateRm> context, CancellationToken token)
    {
        var id = execDateRm.InstructionId;
        if (id < 1)
        {
            context.AddFailure(nameof(ExecDateRm.InstructionId), "Идентификатор поручения не может быть пустым");
            return;
        }

        var instruction = await _instructionsRepository.GetInstructionTreeAsync(id);
        if (instruction is null)
        {
            context.AddFailure(nameof(ExecDateRm.InstructionId), $"Поручение с идентификатором {id} не найдено");
            return;
        }

        if (instruction.ExecutorId != _executorId)
        {
            context.AddFailure(nameof(ExecDateRm.InstructionId), "Вы не являетесь исполнителем этого поручения");
            return;
        }

        if (instruction.ExecDate is not null)
        {
            context.AddFailure(nameof(ExecDateRm.InstructionId), "Поручение уже исполнено");
            return;
        }

        if (_statusService.AnyChildInWork(instruction))
        {
            context.AddFailure(nameof(ExecDateRm.InstructionId), "У поручения есть не исполненные дочерние поручения");
            return;
        }

        if (_statusService.GetMaxChildrenExecDate(instruction) > execDateRm.ExecDate)
        {
            context.AddFailure(nameof(ExecDateRm.ExecDate), "Дата исполнения должна быть больше или равна дат исполнения дочерних поручений");
            return;
        }
    }
}
