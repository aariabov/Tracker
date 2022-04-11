using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tracker.Common;
using Tracker.Db;
using Tracker.Db.Models;
using Tracker.Instructions.RequestModels;

namespace Tracker.Instructions.Validators;

public class InstructionValidator : AbstractValidator<InstructionRm>
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContext;
    
    public InstructionValidator(AppDbContext db, UserManager<User> userManager
        , IHttpContextAccessor httpContext)
    {
        _db = db;
        _userManager = userManager;
        _httpContext = httpContext;

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
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Дедлайн должен быть больше или равно сегодня");
        RuleFor(instruction => instruction.ParentId)
            .Cascade(CascadeMode.Stop)
            .MustAsync(ParentExistsAsync).WithMessage("Родительское поручение не найдено")
            .MustAsync(BeNotExecutedAsync).WithMessage("Родительское поручение уже исполнено")
            .MustAsync(BeCreatedByMyBossAsync).WithMessage("Родительское поручение должно быть создано Вашим руководителем")
            .When(instruction => instruction.ParentId is not null);
    }
    
    private async Task<bool> ExecutorExistsAsync(string executorId, CancellationToken token)
    {
        return await _userManager.Users.AnyAsync(u => u.Id == executorId, token);
    }
    
    private async Task<bool> ParentExistsAsync(int? parentId, CancellationToken token)
    {
        return await _db.Instructions.AnyAsync(u => u.Id == parentId, token);
    }
    
    private async Task<bool> BeNotExecutedAsync(int? parentId, CancellationToken token)
    {
        var allInstructions = await _db.Instructions.ToArrayAsync(token);
        var status = allInstructions.Single(i => i.Id == parentId).Status;
        return status is ExecStatus.InWork or ExecStatus.InWorkOverdue;
    }
    
    private async Task<bool> BeCreatedByMyBossAsync(int? parentId, CancellationToken token)
    {
        var currentUser = await _userManager.GetUserAsync(_httpContext.HttpContext?.User);
        var parentInstruction = _db.Instructions.Single(i => i.Id == parentId);
        return parentInstruction.CreatorId == currentUser.BossId &&
               parentInstruction.ExecutorId == currentUser.Id;
    }
}