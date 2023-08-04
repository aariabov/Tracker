using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Riabov.Tracker.Common.Progress;
using Tracker.Analytics.Db;
using Tracker.Db;
using Tracker.Db.Models;
using Tracker.Db.Transactions;
using Tracker.Db.UnitOfWorks;
using Tracker.Instructions;
using Tracker.Instructions.Db;
using Tracker.Instructions.Generator;
using Tracker.Instructions.Interfaces;
using Tracker.Instructions.Repositories;
using Tracker.Instructions.Validators;
using Tracker.Roles;
using Tracker.Roles.Validators;
using Tracker.Users;
using Tracker.Users.Validators;
using Tracker.Web;
using Tracker.Web.Sandbox;

var builder = WebApplication.CreateBuilder(args);

var instructionsConnectionString = builder.Configuration.GetConnectionString("InstructionsConnection");
builder.Services.AddDbContext<InstructionsDbContext>(options => options.UseNpgsql(instructionsConnectionString).UseSnakeCaseNamingConvention());

var analyticsConnectionString = builder.Configuration.GetConnectionString("AnalyticsConnection");
builder.Services.AddDbContext<AnalyticsDbContext>(options => options.UseNpgsql(analyticsConnectionString).UseSnakeCaseNamingConvention());

var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
builder.Services.AddIdentityCore<User>(o =>
{
    o.Password.RequireDigit = false;
    o.Password.RequireLowercase = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequiredLength = 1;
    o.User.AllowedUserNameCharacters = null;
}).AddRoles<Role>()
    .AddRoleManager<RoleManager<Role>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager<SignInManager<User>>();

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:TokenKey"]));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateAudience = false,
                ValidateIssuer = false,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

builder.Services.AddControllers(option =>
{
    option.EnableEndpointRouting = false;
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    option.Filters.Add(new AuthorizeFilter(policy));
}).ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true; // ручная валидация в контроллерах
});

builder.Services.AddHttpClient();

builder.Services.AddTransient<JwtGenerator>();

builder.Services.AddScoped<IInstructionsService, InstructionsService>();
builder.Services.AddScoped<InstructionSlowGeneratorService>();
builder.Services.AddScoped<InstructionGenerator>();
builder.Services.AddScoped<InstructionsGenerationService>();
builder.Services.AddScoped<IInstructionStatusService, InstructionStatusService>();
builder.Services.AddScoped<IInstructionsRepository, InstructionsRepository>();
// builder.Services.AddScoped<IInstructionsTreeRepository, InstructionsTreeRepositoryCte>();
// builder.Services.AddScoped<IInstructionsTreeRepository, InstructionsTreeRepositoryPath>();
builder.Services.AddScoped<IInstructionsTreeRepository, InstructionsTreeRepositoryClosure>();
builder.Services.AddScoped<InstructionValidationService>();
builder.Services.AddScoped<Tracker.Instructions.UserRepository>();

builder.Services.AddScoped<RolesService>();
builder.Services.AddScoped<RoleValidationService>();
builder.Services.AddScoped<IRoleRepo, RoleRepo>();

builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<TokensService>();
builder.Services.AddScoped<IUserValidationService, UserValidationService>();
builder.Services.AddScoped<IUserManagerService, UserManagerService>();
builder.Services.AddScoped<IUserRepository, Tracker.Users.UserRepository>();

builder.Services.AddScoped<IAuditWebService, AuditWebService>();

builder.Services.AddScoped<Progress>();
builder.Services.AddScoped<ProgressableTestService>();
builder.Services.AddScoped<ProgressableParamsTestService>();
builder.Services.AddScoped<TreePathsService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<RequireValueTypePropertiesSchemaFilter>(true);
    options.SupportNonNullableReferenceTypes();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using var serviceScope = app.Services.CreateScope();

var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
var dataSeeder = serviceScope.ServiceProvider.GetRequiredService<DataSeeder>();
await dbContext.Database.MigrateAsync();
await dataSeeder.Seed();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}").RequireAuthorization();

app.UseExceptionHandler(app.Environment.IsDevelopment() ? "/error-development" : "/error");
app.MapHub<ProgressHub>("/api/progress-hub");
app.Run();

public partial class Program { }
