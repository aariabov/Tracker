using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Tracker.Db;
using Xunit;

namespace Tracker.IntegrationTests.Common;

[Collection("Sequential")]
public class TestBase
{
    private readonly HttpClient _httpClient;

    protected TestBase()
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
        ClearDb(db);
        
        _httpClient = application.CreateClient();
    }

    protected async Task<TResult> GetAsync<TResult>(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TResult>(responseBody);
    }

    protected async Task<TResult> PostAsync<TResult>(string url, object model)
    {
        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url, UriKind.Relative),
            Content = content
        };
            
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
            
        var resultStr = await response.Content.ReadAsStringAsync();
        if (typeof(TResult) == typeof(string))
            return (TResult)(object)resultStr;
            
        return JsonConvert.DeserializeObject<TResult>(resultStr);
    }

    protected async Task<string> PostAsync(string url, object model)
    {
        return await PostAsync<string>(url, model);
    }

    private void ClearDb(AppDbContext db)
    {
        db.Database.ExecuteSqlRaw("delete from asp_net_users where email != 'admin@parma.ru'");
        db.Database.ExecuteSqlRaw("delete from asp_net_roles where name != 'Admin'");
    }
}