using System.Text;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Riabov.Tracker.Common;
using Riabov.Tracker.Common.Progress;
using Tracker.Db;
using Tracker.Db.Models;
using Tracker.Db.Transactions;
using Tracker.Db.UnitOfWorks;
using Tracker.Roles;
using Tracker.Roles.Validators;
using Tracker.Users;
using Tracker.Users.Kafka;
using Tracker.Users.Validators;
using Tracker.Web;
using Tracker.Web.Sandbox;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddJwtAuthentication();

builder.Services.AddControllers(option =>
{
    option.EnableEndpointRouting = false;
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    //option.Filters.Add(new AuthorizeFilter(policy));
}).ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true; // ручная валидация в контроллерах
});

builder.Services.AddHttpClient();

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddTransient<JwtGenerator>();

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

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddSignalR();

builder.Services.AddSingleton<KafkaClientHandle>();
builder.Services.AddSingleton<IProducer, KafkaProducer>();

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
app.MapHub<ProgressHub>("progress-hub");
app.Run();

public partial class Program { }
