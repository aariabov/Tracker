using Microsoft.EntityFrameworkCore;
using Tracker.Instructions;
using Tracker.Instructions.Db;

var builder = WebApplication.CreateBuilder(args);
var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);
var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var instructionsDbContext = serviceScope.ServiceProvider.GetRequiredService<InstructionsDbContext>();
await instructionsDbContext.Database.MigrateAsync();

startup.Configure(app, app.Environment);
app.Run();

public partial class Program { }
