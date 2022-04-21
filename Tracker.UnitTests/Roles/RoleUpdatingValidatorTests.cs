using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Tracker.Common;
using Tracker.Db.Models;
using Tracker.Roles;
using Tracker.Roles.RequestModels;
using Tracker.Roles.Validators;
using Xunit;
using static Tracker.Roles.Validators.FluentValidators.RoleCreationFluentValidator;

namespace Tracker.UnitTests.Roles;

public class RoleUpdatingValidatorTests
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
        var myConfig = new Dictionary<string, string>
        {
            {"DefaultAdmin:Role", "Admin"}
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(myConfig).Build();
        var stubRoleManager = new Mock<IRoleRepo>();
        var roleUpdatingModel = new RoleUpdatingRm { Id = roleId, Name = "4242" };
        var expected = Result.Errors<string>(new Dictionary<string, string>
        {
            {"name", "Идентификатор роли не может быть пустым"}
        });
        var sut = new RoleValidationService(stubRoleManager.Object, config);
        
        var result = await sut.ValidateUpdatingModelAsync(roleUpdatingModel);
        result.Should().BeEquivalentTo(expected);
    }
    
    [Theory]
    [MemberData(nameof(InvalidRoleNames))]
    public async Task validation_error_msg_when_invalid_role_name(string roleName, string errorMsg)
    {
        var myConfig = new Dictionary<string, string>
        {
            {"DefaultAdmin:Role", "Admin"}
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(myConfig).Build();
        var stubRoleManager = new Mock<IRoleRepo>();
        var roleUpdatingModel = new RoleUpdatingRm { Id = "42", Name = roleName };
        var expected = Result.Errors<string>(new Dictionary<string, string>
        {
            {"name", errorMsg}
        });
        var sut = new RoleValidationService(stubRoleManager.Object, config);
        
        var result = await sut.ValidateUpdatingModelAsync(roleUpdatingModel);
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task validation_error_msg_when_role_name_exists_already()
    {
        var myConfig = new Dictionary<string, string>
        {
            {"DefaultAdmin:Role", "Admin"}
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(myConfig).Build();
        const string existingRoleName = "4242";
        var stubRoleManager = new Mock<IRoleRepo>();
        stubRoleManager
            .Setup(m => m.RoleExistsAsync(existingRoleName))
            .ReturnsAsync(true);
        
        var roleUpdatingModel = new RoleUpdatingRm { Id = "42", Name = existingRoleName };
        var expected = Result.Errors<string>(new Dictionary<string, string>
        {
            {"name", "Название роли уже существует"}
        });
        var sut = new RoleValidationService(stubRoleManager.Object, config);
        
        var result = await sut.ValidateUpdatingModelAsync(roleUpdatingModel);
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task validation_error_msg_when_role_not_found()
    {
        var myConfig = new Dictionary<string, string>
        {
            {"DefaultAdmin:Role", "Admin"}
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(myConfig).Build();
        const string roleName = "4242";
        const string roleId = "42";
        var stubRoleManager = new Mock<IRoleRepo>();
        stubRoleManager
            .Setup(m => m.RoleExistsAsync(roleName))
            .ReturnsAsync(false);
        stubRoleManager
            .Setup(m => m.GetRoleById(roleId))
            .ReturnsAsync((Role?)null);
        
        var roleUpdatingModel = new RoleUpdatingRm { Id = roleId, Name = roleName };
        var expected = Result.Errors<string>(new Dictionary<string, string>
        {
            {"name", $"Роль с идентификатором {roleId} не найдена"}
        });
        var sut = new RoleValidationService(stubRoleManager.Object, config);
        
        var result = await sut.ValidateUpdatingModelAsync(roleUpdatingModel);
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task validation_error_msg_when_attempt_to_edit_admin_role()
    {
        var myConfig = new Dictionary<string, string>
        {
            {"DefaultAdmin:Role", "Admin"}
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(myConfig).Build();
        const string roleName = "Admin";
        const string roleId = "42";
        var stubRoleManager = new Mock<IRoleRepo>();
        stubRoleManager
            .Setup(m => m.RoleExistsAsync(roleName))
            .ReturnsAsync(false);
        stubRoleManager
            .Setup(m => m.GetRoleById(roleId))
            .ReturnsAsync(new Role(roleName));
        
        var roleUpdatingModel = new RoleUpdatingRm { Id = roleId, Name = roleName };
        var expected = Result.Errors<string>(new Dictionary<string, string>
        {
            {"name", "Роль 'Admin' нельзя редактировать"}
        });
        var sut = new RoleValidationService(stubRoleManager.Object, config);
        
        var result = await sut.ValidateUpdatingModelAsync(roleUpdatingModel);
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task validation_successful()
    {
        var myConfig = new Dictionary<string, string>
        {
            {"DefaultAdmin:Role", "Admin"}
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(myConfig).Build();
        const string roleName = "4242";
        const string roleId = "42";
        var stubRoleManager = new Mock<IRoleRepo>();
        stubRoleManager
            .Setup(m => m.RoleExistsAsync(roleName))
            .ReturnsAsync(false);
        stubRoleManager
            .Setup(m => m.GetRoleById(roleId))
            .ReturnsAsync(new Role(roleName));
        
        var roleUpdatingModel = new RoleUpdatingRm { Id = roleId, Name = roleName };
        var expected = Result.Ok();
        var sut = new RoleValidationService(stubRoleManager.Object, config);
        
        var result = await sut.ValidateUpdatingModelAsync(roleUpdatingModel);
        result.Should().BeEquivalentTo(expected);
    }
}