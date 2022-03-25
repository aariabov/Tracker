using System.Security.Claims;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Db;
using Tracker.Web.Domain;
using Tracker.Web.RequestModels;

namespace Tracker.Web.Validators;

public class ExecDateValidator: AbstractValidator<ExecDateRm>
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpContext;
    
    public ExecDateValidator(AppDbContext db, IHttpContextAccessor httpContext)
    {
        _db = db;
        _httpContext = httpContext;
        RuleFor(rm => rm.InstructionId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Идентификатор поручения не может быть пустым")
            .CustomAsync(MustBeValidInstruction);
        RuleFor(rm => rm.ExecDate)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Дата исполнения не может быть пустой")
            .Must(BeAValidDate).WithMessage("Дата исполнения должена быть правильной датой")
            .Must(execDate => execDate.Value.Date == DateTime.Today).WithMessage("Дата исполнения должна быть сегодня");
    }
    
    private bool BeAValidDate(DateTime? date)
    {
        return !date.Equals(default(DateTime));
    }
    
    private async Task MustBeValidInstruction(int? id, ValidationContext<ExecDateRm> context, CancellationToken token)
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

        if (!instruction.CanBeExecuted(userId))
        {
            context.AddFailure("Поручение не может быть исполнено");
            return;
        }
    }
}