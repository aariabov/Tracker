using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Riabov.Tracker.Common;
using Riabov.Tracker.Common.Progress;
using Tracker.Instructions.Db;
using Tracker.Instructions.Generator;
using Tracker.Instructions.Interfaces;
using Tracker.Instructions.Kafka;
using Tracker.Instructions.Repositories;
using Tracker.Instructions.Validators;

namespace Tracker.Instructions;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add serices to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        var instructionsConnectionString = Configuration.GetConnectionString("InstructionsConnection");
        services.AddDbContext<InstructionsDbContext>(options => options.UseNpgsql(instructionsConnectionString).UseSnakeCaseNamingConvention());

        services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
        });

        // Add services to the container.
        services.AddScoped<InstructionsService>();
        services.AddScoped<InstructionSlowGeneratorService>();
        services.AddScoped<InstructionGenerator>();
        services.AddScoped<InstructionsGenerationService>();
        services.AddScoped<InstructionStatusService>();
        services.AddScoped<InstructionsRepository>();
        // Services.AddScoped<IInstructionsTreeRepository, InstructionsTreeRepositoryCte>();
        // Services.AddScoped<IInstructionsTreeRepository, InstructionsTreeRepositoryPath>();
        services.AddScoped<IInstructionsTreeRepository, InstructionsTreeRepositoryClosure>();
        services.AddScoped<InstructionValidationService>();
        services.AddScoped<TreePathsService>();
        services.AddScoped<UserRepository>();
        services.AddScoped<UserService>();
        services.AddScoped<ReCalcStatusService>();
        services.AddHttpContextAccessor();

        services.AddJwtAuthentication();

        services.AddScoped<Progress>();
        services.AddSignalR();

        services.AddSingleton<KafkaClientHandle>();
        services.AddHostedService<KafkaUserConsumer>();
        services.AddHostedService<ReCalcStatusBackgroundService>();
        services.AddSingleton<IProducer, KafkaProducer>();

        services
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SchemaFilter<RequireValueTypePropertiesSchemaFilter>(true);
            options.SupportNonNullableReferenceTypes();
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
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
        //app.MapHub<ProgressHub>("progress-hub");
    }
}
