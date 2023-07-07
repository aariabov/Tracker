using Microsoft.Extensions.Configuration;

namespace Tracker.Users;

public partial class AuditWebService : IAuditWebService
{
    public AuditWebService(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        var section = config.GetSection("WebServices");
        var auditWebServiceUrl = section["AuditWebService"];

        BaseUrl = auditWebServiceUrl;
        _httpClient = httpClientFactory.CreateClient();
        _settings = new Lazy<Newtonsoft.Json.JsonSerializerSettings>(CreateSerializerSettings);
    }
}
