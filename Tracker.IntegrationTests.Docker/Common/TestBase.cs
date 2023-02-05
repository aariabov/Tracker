using System.Configuration;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using Xunit;

namespace Tracker.IntegrationTests.Docker.Common;

[Collection("Sequential")]
public class TestBase : IDisposable
{
    private readonly HttpClient _httpClient;

    protected TestBase()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Testing.Docker.json", optional: false, reloadOnChange: true)
            .Build();

        var backendUrl = config["BackendUrl"] ?? throw new ConfigurationErrorsException("BackendUrl is missing");
        var connectionString = config.GetConnectionString("PostgresConnection") ??
                               throw new ConfigurationErrorsException("PostgresConnection is missing");

        ClearDb(connectionString);

        var handler = new HttpClientHandler();
        handler.UseDefaultCredentials = true;
        _httpClient = new HttpClient(handler);
        _httpClient.BaseAddress = new Uri(backendUrl);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Integration tests");
    }

    protected async Task<TResult> GetAsync<TResult>(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TResult>(responseBody);
    }

    protected async Task<TResult> PostAsync<TResult>(string url, object model, string? token)
    {
        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url, UriKind.Relative),
            Content = content
        };

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var resultStr = await response.Content.ReadAsStringAsync();
        if (typeof(TResult) == typeof(string))
        {
            return (TResult)(object)resultStr;
        }

        return JsonConvert.DeserializeObject<TResult>(resultStr);
    }

    protected async Task<string> PostAsync(string url, object model, string? token)
    {
        return await PostAsync<string>(url, model, token);
    }

    private void ClearDb(string connectionString)
    {
        using var sc = new NpgsqlConnection(connectionString);
        using var cmd = sc.CreateCommand();
        sc.Open();
        cmd.CommandText = "delete from asp_net_users where email != 'admin@parma.ru'";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "delete from asp_net_roles where name != 'Admin'";
        cmd.ExecuteNonQuery();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
