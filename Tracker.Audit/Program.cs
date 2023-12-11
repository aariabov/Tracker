using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Riabov.Tracker.Common;
using Tracker.Audit;
using Tracker.Audit.Db;

var builder = WebApplication.CreateBuilder(args);

var auditConnectionString = builder.Configuration.GetConnectionString("AuditConnection");
builder.Services.AddDbContext<AuditDbContext>(options => options.UseNpgsql(auditConnectionString).UseSnakeCaseNamingConvention());

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

// Add services to the container.
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<AuditRepository>();

builder.Services.AddJwtAuthentication();

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var auditDbContext = serviceScope.ServiceProvider.GetRequiredService<AuditDbContext>();
await auditDbContext.Database.MigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
