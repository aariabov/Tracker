using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Riabov.Tracker.Common;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("ocelot.json")
    .Build();

builder.Services.AddOcelot(configuration);
builder.Services.AddJwtAuthentication();

var app = builder.Build();
app.UseWebSockets();
await app.UseOcelot();

app.MapGet("/", () => "Hello World!");

app.UseAuthentication();
app.UseAuthorization();
app.Run();
