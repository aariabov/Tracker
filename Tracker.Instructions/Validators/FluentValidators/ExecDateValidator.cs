using FluentValidation;
using Tracker.Instructions.RequestModels;
using Tracker.Users;

namespace Tracker.Instructions.Validators.FluentValidators;

internal class ExecDateValidator: AbstractValidator<ExecDateRm>
{
    private readonly IInstructionsRepository _instructionsRepository;
    private readonly UsersService _usersService;
    private readonly IInstructionStatusService _statusService;
    
    public ExecDateValidator(IInstructionsRepository instructionsRepository
        , UsersService usersService
        , IInstructionStatusService statusService)
    {
        _instructionsRepository = instructionsRepository;
        _usersService = usersService;
        _statusService = statusService;

        RuleFor(rm => rm.InstructionId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Идентификатор поручения не может быть пустым")
            .CustomAsync(MustBeValidInstruction);
        RuleFor(rm => rm.ExecDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Дата исполнения не может быть пустой")
            .Must(execDate => execDate.Date == DateTime.UtcNow.Date).WithMessage("Дата исполнения должна быть сегодня");
    }
    
    private async Task MustBeValidInstruction(int id, ValidationContext<ExecDateRm> context, CancellationToken token)
    {
        var userId = _usersService.GetCurrentUserId();
        var allInstructions = await _instructionsRepository.GetAllInstructionsAsync();
        var instruction = allInstructions.SingleOrDefault(i => i.Id == id);
        if (instruction is null)
        {
            context.AddFailure($"Поручение с идентификатором {id} не найдено");
            return;
        }

        if (instruction.ExecutorId != userId)
        {
            context.AddFailure("Вы не являетесь исполнителем этого поручения");
            return;
        }

        if (instruction.ExecDate is not null)   
        {
            context.AddFailure("Поручение уже исполнено");
            return;
        }

        if (_statusService.AnyChildInWork(instruction))
        {
            context.AddFailure("У поручения есть не исполненные дочерние поручения");
            return;
        }
    }
}