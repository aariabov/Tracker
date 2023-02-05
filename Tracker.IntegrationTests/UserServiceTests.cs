using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Tracker.Common;
using Tracker.IntegrationTests.Common;
using Tracker.Users.RequestModels;
using Xunit;

namespace Tracker.IntegrationTests;

public class UserServiceTests : TestBase
{
    [Fact]
    public async Task success_creation_updating_and_deleting_user()
    {
        // регистрация пользователя
        var registrationModel = new UserRegistrationRm
        {
            Name = "test",
            Email = "test@parma.ru",
            Password = "1"
        };

        var userId = await PostAsync("api/users/register", registrationModel);
        Guid.TryParse(userId, out _).Should().BeTrue();

        // повторная регистрация пользователя - ошибка валидации
        var errorsModel = await PostAsync<ModelErrorsVm>("api/users/register", registrationModel);
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
        var updatingResponse = await PostAsync("api/users/update", updatingModel);
        updatingResponse.Should().BeEmpty();

        // удаление пользователя
        var userDeletingModel = new UserDeletingRm
        {
            Id = userId
        };
        var deletingResponse = await PostAsync("api/users/delete", userDeletingModel);
        deletingResponse.Should().BeEmpty();
    }
}
