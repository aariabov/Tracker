using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Riabov.Tracker.Common;
using Tracker.Roles;
using Tracker.Roles.RequestModels;
using Tracker.Roles.Validators;
using Xunit;
using static Tracker.Roles.Validators.FluentValidators.RoleCreationFluentValidator;

namespace Tracker.UnitTests.Roles;

public class RoleCreationValidatorTests
{
    public static IEnumerable<object?[]> InvalidRoleNames()
    {
        var tooShortName = new string('*', RoleMinLen - 1);
        var tooLongName = new string('*', RoleMaxLen + 1);

        yield return new object?[] { null, "Название роли не может быть пустым" };
        yield return new object?[] { string.Empty, "Название роли не может быть пустым" };
        yield return new object?[] { "    ", "Название роли не может быть пустым" };
        yield return new object?[] { tooShortName, $"Название роли должно быть от {RoleMinLen} до {RoleMaxLen} символов" };
        yield return new object?[] { tooLongName, $"Название роли должно быть от {RoleMinLen} до {RoleMaxLen} символов" };
    }

    [Theory]
    [MemberData(nameof(InvalidRoleNames))]
    public async Task validation_error_msg_when_invalid_role_name(string roleName, string errorMsg)
    {
        var roleCreationModel = new RoleCreationRm { Name = roleName };
        var expected = Result.Errors<string>(new Dictionary<string, string>
        {
            {"name", errorMsg}
        });
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var sut = fixture.Create<RoleValidationService>();

        var result = await sut.ValidateCreationModelAsync(roleCreationModel);
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task validation_error_msg_when_role_name_exists_already()
    {
        const string existingRoleName = "4242";
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var stubRoleManager = fixture.Freeze<Mock<IRoleRepo>>();
        stubRoleManager
            .Setup(m => m.RoleExistsAsync(existingRoleName))
            .ReturnsAsync(true);

        var roleCreationModel = new RoleCreationRm { Name = existingRoleName };
        var expected = Result.Errors<string>(new Dictionary<string, string>
        {
            {"name", "Название роли уже существует"}
        });
        var sut = fixture.Create<RoleValidationService>();

        var result = await sut.ValidateCreationModelAsync(roleCreationModel);
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task validation_successful()
    {
        const string roleName = "4242";
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var stubRoleManager = fixture.Freeze<Mock<IRoleRepo>>();
        stubRoleManager
            .Setup(m => m.RoleExistsAsync(roleName))
            .ReturnsAsync(false);

        var roleCreationModel = new RoleCreationRm { Name = roleName };
        var expected = Result.Ok();
        var sut = fixture.Create<RoleValidationService>();

        var result = await sut.ValidateCreationModelAsync(roleCreationModel);
        result.Should().BeEquivalentTo(expected);
    }
}
