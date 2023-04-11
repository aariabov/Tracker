using FluentAssertions;
using Tracker.IntegrationTests.Docker.Common;
using Tracker.Roles;
using Tracker.Roles.Common;
using Tracker.Roles.RequestModels;
using Tracker.Users.ViewModels;
using Xunit;

namespace Tracker.IntegrationTests.Docker;

public class RolesTests : TestBase
{
    [Fact]
    public async Task concurrency_role_updating()
    {
        // login
        var loginVm = new LoginVM
        {
            Email = "admin@parma.ru",
            Password = "1"
        };
        var tokensVm = await PostAsync<TokensVm>("/api/users/login", loginVm, token: null);

        // создание роли
        const string testRole = "test role";
        var roleCreationRm = new RoleCreationRm
        {
            Name = testRole
        };

        var roleId = await PostAsync("/api/roles/create", roleCreationRm, tokensVm.Token);
        Guid.TryParse(roleId, out _).Should().BeTrue();

        // получение роли
        var existingRoles = await GetAsync<RoleVm[]>("api/roles");
        var existingRole = existingRoles.Single(r => r.Name == testRole);

        // обновление роли
        var roleUpdatingRm = new RoleUpdatingRm
        {
            Id = existingRole.Id,
            Name = "new role name",
            ConcurrencyStamp = existingRole.ConcurrencyStamp
        };
        var updatingResponse = await PostAsync("api/roles/update", roleUpdatingRm, tokensVm.Token);
        updatingResponse.Should().BeEmpty();

        // конкурентное обновление роли - ошибка
        var roleUpdatingRm1 = new RoleUpdatingRm
        {
            Id = existingRole.Id,
            Name = "new role name1",
            ConcurrencyStamp = existingRole.ConcurrencyStamp
        };
        var errorsModel = await PostAsync<RoleModelErrorsVm>("api/roles/update", roleUpdatingRm1, tokensVm.Token);
        var expectedModel = new RoleModelErrorsVm(Result.Errors<string>(new Dictionary<string, string>
        {
            { "name", "Роль была изменена, обновите страницу и попробуйте заново" }
        }));
        errorsModel.Should().BeEquivalentTo(expectedModel);
    }
}
