using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Riabov.Tracker.Common;
using Tracker.Db.Models;
using Tracker.Roles;
using Tracker.Roles.RequestModels;
using Tracker.Roles.Validators;
using Xunit;

namespace Tracker.UnitTests.Roles;

public class RoleDeletingValidatorTests
{
    public static IEnumerable<object?[]> InvalidRoleIds()
    {
        yield return new object?[] { null };
        yield return new object?[] { string.Empty };
        yield return new object?[] { "    " };
    }

    [Theory]
    [MemberData(nameof(InvalidRoleIds))]
    public async Task validation_error_msg_when_role_id_is_empty(string roleId)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var roleDeletingModel = new RoleDeletingRm { Id = roleId };
        var expected = Result.Errors<string>(new Dictionary<string, string>
        {
            {"id", "Идентификатор роли не может быть пустым"}
        });
        var sut = fixture.Create<RoleValidationService>();

        var result = await sut.ValidateDeletingModelAsync(roleDeletingModel);
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task validation_error_msg_when_role_not_found()
    {
        const string roleId = "42";
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var stubRoleManager = fixture.Freeze<Mock<IRoleRepo>>();
        stubRoleManager
            .Setup(m => m.GetRoleById(roleId))
            .ReturnsAsync((Role?)null);

        var roleDeletingModel = new RoleDeletingRm { Id = roleId };
        var expected = Result.Errors<string>(new Dictionary<string, string>
        {
            {"id", $"Роль с идентификатором {roleId} не найдена"}
        });
        var sut = fixture.Create<RoleValidationService>();

        var result = await sut.ValidateDeletingModelAsync(roleDeletingModel);
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task validation_error_msg_when_users_use_role()
    {
        const string roleId = "42";
        const string roleName = "4242";
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var stubRoleManager = fixture.Freeze<Mock<IRoleRepo>>();
        stubRoleManager
            .Setup(m => m.GetRoleById(roleId))
            .ReturnsAsync(new Role(roleName));
        stubRoleManager
            .Setup(m => m.IsAnyUserBelongToRole(roleId))
            .ReturnsAsync(true);

        var roleDeletingModel = new RoleDeletingRm { Id = roleId };
        var expected = Result.Errors<string>(new Dictionary<string, string>
        {
            {"id", $"Пользователи используют роль '{roleName}'"}
        });
        var sut = fixture.Create<RoleValidationService>();

        var result = await sut.ValidateDeletingModelAsync(roleDeletingModel);
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task validation_successful()
    {
        const string roleId = "42";
        const string roleName = "4242";
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var stubRoleManager = fixture.Freeze<Mock<IRoleRepo>>();
        stubRoleManager
            .Setup(m => m.GetRoleById(roleId))
            .ReturnsAsync(new Role(roleName));
        stubRoleManager
            .Setup(m => m.IsAnyUserBelongToRole(roleId))
            .ReturnsAsync(false);

        var roleDeletingModel = new RoleDeletingRm { Id = roleId };
        var expected = Result.Ok();
        var sut = fixture.Create<RoleValidationService>();

        var result = await sut.ValidateDeletingModelAsync(roleDeletingModel);
        result.Should().BeEquivalentTo(expected);
    }
}
