using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Tracker.Instructions;
using Tracker.Instructions.Db;
using Tracker.Instructions.Generator;
using Tracker.Instructions.Interfaces;
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


builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
