using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Riabov.Tracker.Common;
using Riabov.Tracker.Common.Progress;
using Tracker.Instructions;
using Tracker.Instructions.Db;
using Tracker.Instructions.Generator;
using Tracker.Instructions.Interfaces;
using Tracker.Instructions.Kafka;
using Tracker.Instructions.Repositories;
using Tracker.Instructions.Validators;
using UserRepository = Tracker.Instructions.UserRepository;

var builder = WebApplication.CreateBuilder(args);

var instructionsConnectionString = builder.Configuration.GetConnectionString("InstructionsConnection");
builder.Services.AddDbContext<InstructionsDbContext>(options => options.UseNpgsql(instructionsConnectionString).UseSnakeCaseNamingConvention());


// Add services to the container.
builder.Services.AddScoped<InstructionsService>();
builder.Services.AddScoped<InstructionSlowGeneratorService>();
builder.Services.AddScoped<InstructionGenerator>();
builder.Services.AddScoped<InstructionsGenerationService>();
builder.Services.AddScoped<InstructionStatusService>();
builder.Services.AddScoped<InstructionsRepository>();
// builder.Services.AddScoped<IInstructionsTreeRepository, InstructionsTreeRepositoryCte>();
// builder.Services.AddScoped<IInstructionsTreeRepository, InstructionsTreeRepositoryPath>();
builder.Services.AddScoped<IInstructionsTreeRepository, InstructionsTreeRepositoryClosure>();
builder.Services.AddScoped<InstructionValidationService>();
builder.Services.AddScoped<TreePathsService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddJwtAuthentication();

builder.Services.AddScoped<Progress>();
builder.Services.AddSignalR();

builder.Services.AddHostedService<KafkaUserConsumer>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<RequireValueTypePropertiesSchemaFilter>(true);
    options.SupportNonNullableReferenceTypes();
});

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var instructionsDbContext = serviceScope.ServiceProvider.GetRequiredService<InstructionsDbContext>();
await instructionsDbContext.Database.MigrateAsync();

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
app.MapHub<ProgressHub>("/api/progress-hub");
app.Run();
