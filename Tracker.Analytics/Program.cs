using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Riabov.Tracker.Common;
using Tracker.Analytics.Db;
using Tracker.Analytics.Instructions;
using Tracker.Analytics.Kafka;
using Tracker.Analytics.Users;

var builder = WebApplication.CreateBuilder(args);

var analyticsConnectionString = builder.Configuration.GetConnectionString("AnalyticsConnection");
builder.Services.AddDbContext<AnalyticsDbContext>(options => options.UseNpgsql(analyticsConnectionString).UseSnakeCaseNamingConvention());

builder.Services.AddHostedService<KafkaUserConsumer>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<InstructionService>();
builder.Services.AddScoped<InstructionRepository>();

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtAuthentication();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
