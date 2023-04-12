using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Riabov.Tracker.Common;
using Tracker.IntegrationTests.Common;
using Tracker.Roles;
using Tracker.Roles.RequestModels;
using Xunit;

namespace Tracker.IntegrationTests;

public class RolesTests : TestBase
{
    [Fact]
    public async Task concurrency_role_updating()
    {
        // создание роли
        const string testRole = "test role";
        var roleCreationRm = new RoleCreationRm
        {
            Name = testRole
        };

        var roleId = await PostAsync("api/roles/create", roleCreationRm);
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
        var updatingResponse = await PostAsync("api/roles/update", roleUpdatingRm);
        updatingResponse.Should().BeEmpty();

        // конкурентное обновление роли - ошибка
        var roleUpdatingRm1 = new RoleUpdatingRm
        {
            Id = existingRole.Id,
            Name = "new role name1",
            ConcurrencyStamp = existingRole.ConcurrencyStamp
        };
        var errorsModel = await PostAsync<ModelErrorsVm>("api/roles/update", roleUpdatingRm1);
        var expectedModel = new ModelErrorsVm(Result.Errors<string>(new Dictionary<string, string>
        {
            { "name", "Роль была изменена, обновите страницу и попробуйте заново" }
        }));
        errorsModel.Should().BeEquivalentTo(expectedModel);
    }
}
