using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Tracker.Db;
using Tracker.Users.RequestModels;

namespace Tracker.Users.Validators.FluentValidators;

public class UserDeletingValidator : AbstractValidator<UserDeletingRm>
{
    private readonly IUserRepository _userRepository;
    private readonly AppDbContext _db;
    private readonly string _adminEmail;

    public UserDeletingValidator(AppDbContext db, IUserRepository userRepository, string adminEmail)
    {
        _db = db;
        _adminEmail = adminEmail;
        _userRepository = userRepository;

        RuleFor(rm => rm.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Идентификатор не может быть пустым")
            .CustomAsync(MustBeValid);
    }

    private async Task MustBeValid(string userId, ValidationContext<UserDeletingRm> context, CancellationToken token)
    {
        var user = await _userRepository.GetUserById(userId);
        if (user is null)
        {
            context.AddFailure($"Пользователь с идентификатором {userId} не найден");
            return;
        }

        var hasChildren = await _userRepository.HasChildren(user.Id);
        if (hasChildren)
        {
            context.AddFailure("У пользователя есть подчиненные");
            return;
        }

        // TODO: возможно, надо проверить есть ли у юзера поручения (запрос в сервис с поручениями)

        if (user.IsAdmin(_adminEmail))
        {
            context.AddFailure("Администратора нельзя удалить");
            return;
        }
    }
}
