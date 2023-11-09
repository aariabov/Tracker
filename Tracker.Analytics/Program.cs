using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Tracker.Analytics.Db;

var builder = WebApplication.CreateBuilder(args);

var analyticsConnectionString = builder.Configuration.GetConnectionString("AnalyticsConnection");
builder.Services.AddDbContext<AnalyticsDbContext>(options => options.UseNpgsql(analyticsConnectionString).UseSnakeCaseNamingConvention());

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var analyticsDbContext = serviceScope.ServiceProvider.GetRequiredService<AnalyticsDbContext>();
await analyticsDbContext.Database.MigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
