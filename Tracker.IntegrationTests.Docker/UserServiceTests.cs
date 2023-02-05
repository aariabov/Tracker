using FluentAssertions;
using Tracker.Common;
using Tracker.IntegrationTests.Docker.Common;
using Tracker.Users.RequestModels;
using Tracker.Users.ViewModels;
using Xunit;

namespace Tracker.IntegrationTests.Docker;

public class UserServiceTests : TestBase
{
    [Fact]
    public async Task success_creation_updating_and_deleting_user()
    {
        // login
        var loginVm = new LoginVM
        {
            Email = "admin@parma.ru",
            Password = "1"
        };
        var tokensVm = await PostAsync<TokensVm>("/api/users/login", loginVm, token: null);

        // регистрация пользователя
        var registrationModel = new UserRegistrationRm
        {
            Name = "test",
            Email = "test@parma.ru",
            Password = "1"
        };

        var userId = await PostAsync("api/users/register", registrationModel, tokensVm.Token);
        Guid.TryParse(userId, out _).Should().BeTrue();

        // повторная регистрация пользователя - ошибка валидации
        var errorsModel = await PostAsync<ModelErrorsVm>("api/users/register", registrationModel, tokensVm.Token);
        var expectedModel = new ModelErrorsVm(Result.Errors<string>(new Dictionary<string, string>
        {
            { "email", "Email уже существует" }
        }));
        errorsModel.Should().BeEquivalentTo(expectedModel);

        // обновление пользователя
        var updatingModel = new UserUpdatingRm
        {
            Id = userId,
            Name = "new name",
            Email = "test@parma.ru"
        };
        var updatingResponse = await PostAsync("api/users/update", updatingModel, tokensVm.Token);
        updatingResponse.Should().BeEmpty();

        // удаление пользователя
        var userDeletingModel = new UserDeletingRm
        {
            Id = userId
        };
        var deletingResponse = await PostAsync("api/users/delete", userDeletingModel, tokensVm.Token);
        deletingResponse.Should().BeEmpty();
    }
}
