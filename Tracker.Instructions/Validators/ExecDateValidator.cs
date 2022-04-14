using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tracker.Db;
using Tracker.Instructions.RequestModels;

namespace Tracker.Instructions.Validators;

public class ExecDateValidator: AbstractValidator<ExecDateRm>
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IInstructionsService _instructionsService;
    
    public ExecDateValidator(AppDbContext db
        , IHttpContextAccessor httpContext
        , IInstructionsService instructionsService)
    {
        _db = db;
        _httpContext = httpContext;
        _instructionsService = instructionsService;
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
        var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var allInstructions = await _db.Instructions.ToArrayAsync(token);
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

        if (!_instructionsService.CanBeExecuted(instruction, userId))
        {
            context.AddFailure("Поручение не может быть исполнено");
            return;
        }
    }
}