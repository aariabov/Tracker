using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tracker.Common;
using Tracker.Db;
using Tracker.Users.RequestModels;
using Xunit;

namespace Tracker.IntegrationTests;

public class UserServiceTests
{
    private readonly HttpClient _client;
    
    public UserServiceTests()
    {
        var application = new TestWebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(
                services =>
                {
                    services.AddMvc(options =>
                        {
                            options.Filters.Add(new AllowAnonymousFilter());
                        });
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => {});
                }));
        
        var db = application.Services.GetRequiredService<AppDbContext>();
        db.Database.ExecuteSqlRaw("delete from asp_net_users where email != 'admin@parma.ru'");
        _client = application.CreateClient();
    }

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

        var userId = await MakeRequest("api/users/register", registrationModel);
        Guid.TryParse(userId, out _).Should().BeTrue();
        
        // повторная регистрация пользователя - ошибка валидации
        var errorsModelJson = await MakeRequest("api/users/register", registrationModel);
        var errorsModel = JsonConvert.DeserializeObject<ModelErrorsVm>(errorsModelJson);
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
        var updatingResponse = await MakeRequest("api/users/update", updatingModel);
        updatingResponse.Should().BeEmpty();

        // удаление пользователя
        var userDeletingModel = new UserDeletingRm
        {
            Id = userId
        };
        var deletingResponse = await MakeRequest("api/users/delete", userDeletingModel);
        deletingResponse.Should().BeEmpty();
    }

    private async Task<string> MakeRequest<T>(string url, T model)
    {
        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url, UriKind.Relative),
            Content = content
        };
            
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
            
        return await response.Content.ReadAsStringAsync();
    }
}

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.Role, "Admin") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}