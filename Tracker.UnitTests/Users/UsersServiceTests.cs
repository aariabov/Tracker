using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Riabov.Tracker.Common;
using Tracker.Db.Models;
using Tracker.Db.Transactions;
using Tracker.Db.UnitOfWorks;
using Tracker.Users;
using Tracker.Users.RequestModels;
using Tracker.Users.Validators;
using Xunit;

namespace Tracker.UnitTests.Users;

public class UsersServiceTests
{
    private const string ADMIN = "admin";

    [Fact]
    public async Task register_successful()
    {
        var validUserRegistrationRm = new UserRegistrationRm
        {
            Name = "test",
            Email = "test@mail.ru",
            Password = "42",
            Roles = new[] { "testRole" }
        };

        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockTransaction = fixture.Freeze<Mock<IContextTransaction>>();
        var mockUnitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
        var mockAuditService = fixture.Freeze<Mock<IAuditWebService>>();
        var mockTransactionManager = fixture.Freeze<Mock<ITransactionManager>>();
        mockTransactionManager
            .Setup(m => m.BeginTransaction(IsolationLevel.Unspecified))
            .Returns(mockTransaction.Object);

        var stubUserValidationService = fixture.Freeze<Mock<IUserValidationService>>();
        stubUserValidationService
            .Setup(s => s.ValidateRegistrationModelAsync(validUserRegistrationRm))
            .ReturnsAsync(Result.Ok());

        var stubHttpContextAccessor = fixture.Freeze<Mock<IHttpContextAccessor>>();
        stubHttpContextAccessor
            .Setup(s => s.HttpContext.User.FindFirst(It.IsAny<string>()))
            .Returns(new Claim("name", ADMIN));

        var stubUserManagerService = fixture.Freeze<Mock<IUserManagerService>>();
        stubUserManagerService
            .Setup(s => s.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        stubUserManagerService
            .Setup(s => s.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

        var expected = Result.Ok();
        var sut = fixture.Create<UsersService>();

        var result = await sut.RegisterAsync(validUserRegistrationRm);
        result.Should().BeEquivalentTo(expected);
        mockTransactionManager.Verify(m => m.BeginTransaction(IsolationLevel.Unspecified), Times.Once);
        mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Once);
        mockTransaction.Verify(m => m.CommitAsync(), Times.Once);
        mockAuditService.Verify(m => m.CreateLogAsync(It.Is<LogModel>(lm =>
                lm.Type == AuditType.Create && lm.EntityName == nameof(User) && lm.UserId == ADMIN)),
            Times.Once);
        mockTransaction.Verify(m => m.RollbackAsync(), Times.Never);
    }

    [Fact]
    public async Task rollback_transaction_when_user_creation_error()
    {
        var validUserRegistrationRm = new UserRegistrationRm
        {
            Name = "test",
            Email = "test@mail.ru",
            Password = "42",
            Roles = new[] { "testRole" }
        };

        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockTransaction = fixture.Freeze<Mock<IContextTransaction>>();
        var mockUnitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
        var mockAuditService = fixture.Freeze<Mock<IAuditWebService>>();
        var mockTransactionManager = fixture.Freeze<Mock<ITransactionManager>>();
        mockTransactionManager
            .Setup(m => m.BeginTransaction(IsolationLevel.Unspecified))
            .Returns(mockTransaction.Object);

        var stubUserValidationService = fixture.Freeze<Mock<IUserValidationService>>();
        stubUserValidationService
            .Setup(s => s.ValidateRegistrationModelAsync(validUserRegistrationRm))
            .ReturnsAsync(Result.Ok());

        var stubUserManagerService = fixture.Freeze<Mock<IUserManagerService>>();
        stubUserManagerService
            .Setup(s => s.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());

        var sut = fixture.Create<UsersService>();

        var act = async () => await sut.RegisterAsync(validUserRegistrationRm);
        await act.Should().ThrowAsync<Exception>();
        mockTransactionManager.Verify(m => m.BeginTransaction(IsolationLevel.Unspecified), Times.Once);
        mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Never);
        mockTransaction.Verify(m => m.CommitAsync(), Times.Never);
        mockAuditService.Verify(m => m.CreateLogAsync(It.Is<LogModel>(lm =>
                lm.Type == AuditType.Create && lm.EntityName == nameof(User) && lm.UserId == ADMIN)),
            Times.Never);
        mockTransaction.Verify(m => m.RollbackAsync(), Times.Once);
    }

    [Fact]
    public async Task rollback_transaction_when_audit_log_error()
    {
        var validUserRegistrationRm = new UserRegistrationRm
        {
            Name = "test",
            Email = "test@mail.ru",
            Password = "42",
            Roles = new[] { "testRole" }
        };

        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockTransaction = fixture.Freeze<Mock<IContextTransaction>>();
        var mockUnitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
        var mockAuditService = fixture.Freeze<Mock<IAuditWebService>>();
        mockAuditService
            .Setup(m => m.CreateLogAsync(It.Is<LogModel>(lm =>
                lm.Type == AuditType.Create && lm.EntityName == nameof(User) && lm.UserId == ADMIN)))
            .Throws<Exception>();

        var mockTransactionManager = fixture.Freeze<Mock<ITransactionManager>>();
        mockTransactionManager
            .Setup(m => m.BeginTransaction(IsolationLevel.Unspecified))
            .Returns(mockTransaction.Object);

        var stubUserValidationService = fixture.Freeze<Mock<IUserValidationService>>();
        stubUserValidationService
            .Setup(s => s.ValidateRegistrationModelAsync(validUserRegistrationRm))
            .ReturnsAsync(Result.Ok());

        var stubHttpContextAccessor = fixture.Freeze<Mock<IHttpContextAccessor>>();
        stubHttpContextAccessor
            .Setup(s => s.HttpContext.User.FindFirst(It.IsAny<string>()))
            .Returns(new Claim("name", ADMIN));

        var stubUserManagerService = fixture.Freeze<Mock<IUserManagerService>>();
        stubUserManagerService
            .Setup(s => s.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        stubUserManagerService
            .Setup(s => s.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

        var sut = fixture.Create<UsersService>();

        var act = async () => await sut.RegisterAsync(validUserRegistrationRm);
        await act.Should().ThrowAsync<Exception>();
        mockTransactionManager.Verify(m => m.BeginTransaction(IsolationLevel.Unspecified), Times.Once);
        mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Never);
        mockTransaction.Verify(m => m.CommitAsync(), Times.Never);
        mockAuditService.Verify(m => m.CreateLogAsync(It.Is<LogModel>(lm =>
                lm.Type == AuditType.Create && lm.EntityName == nameof(User) && lm.UserId == ADMIN)),
            Times.Once);
        mockTransaction.Verify(m => m.RollbackAsync(), Times.Once);
    }

    [Fact]
    public async Task rollback_transaction_when_roles_error()
    {
        const int AUDIT_LOG_ID = 1;

        var validUserRegistrationRm = new UserRegistrationRm
        {
            Name = "test",
            Email = "test@mail.ru",
            Password = "42",
            Roles = new[] { "testRole" }
        };

        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var mockTransaction = fixture.Freeze<Mock<IContextTransaction>>();
        var mockUnitOfWork = fixture.Freeze<Mock<IUnitOfWork>>();
        var mockAuditService = fixture.Freeze<Mock<IAuditWebService>>();
        mockAuditService
            .Setup(m => m.CreateLogAsync(It.Is<LogModel>(lm =>
                lm.Type == AuditType.Create && lm.EntityName == nameof(User) && lm.UserId == ADMIN)))
            .ReturnsAsync(AUDIT_LOG_ID);

        var mockTransactionManager = fixture.Freeze<Mock<ITransactionManager>>();
        mockTransactionManager
            .Setup(m => m.BeginTransaction(IsolationLevel.Unspecified))
            .Returns(mockTransaction.Object);

        var stubUserValidationService = fixture.Freeze<Mock<IUserValidationService>>();
        stubUserValidationService
            .Setup(s => s.ValidateRegistrationModelAsync(validUserRegistrationRm))
            .ReturnsAsync(Result.Ok());

        var stubHttpContextAccessor = fixture.Freeze<Mock<IHttpContextAccessor>>();
        stubHttpContextAccessor
            .Setup(s => s.HttpContext.User.FindFirst(It.IsAny<string>()))
            .Returns(new Claim("name", ADMIN));

        var stubUserManagerService = fixture.Freeze<Mock<IUserManagerService>>();
        stubUserManagerService
            .Setup(s => s.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        stubUserManagerService
            .Setup(s => s.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Failed());

        var sut = fixture.Create<UsersService>();

        var act = async () => await sut.RegisterAsync(validUserRegistrationRm);
        await act.Should().ThrowAsync<Exception>();
        mockTransactionManager.Verify(m => m.BeginTransaction(IsolationLevel.Unspecified), Times.Once);
        mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Never);
        mockTransaction.Verify(m => m.CommitAsync(), Times.Never);
        mockAuditService.Verify(m => m.CreateLogAsync(It.Is<LogModel>(lm =>
                lm.Type == AuditType.Create && lm.EntityName == nameof(User) && lm.UserId == ADMIN)),
            Times.Once);
        mockAuditService.Verify(m => m.DeleteLogAsync(It.Is<DeleteLogModel>(d => d.Id == AUDIT_LOG_ID)), Times.Once);
        mockTransaction.Verify(m => m.RollbackAsync(), Times.Once);
    }
}
