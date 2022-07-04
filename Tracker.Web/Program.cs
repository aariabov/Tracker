using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Tracker.Audit;
using Tracker.Db;
using Tracker.Db.Models;
using Tracker.Db.Transactions;
using Tracker.Db.UnitOfWorks;
using Tracker.Instructions;
using Tracker.Instructions.Interfaces;
using Tracker.Instructions.Repositories;
using Tracker.Instructions.Validators;
using Tracker.Roles;
using Tracker.Roles.Validators;
using Tracker.Users;
using Tracker.Users.Validators;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
builder.Services.AddIdentityCore<User>(o => {
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
    var policy =  new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    option.Filters.Add(new AuthorizeFilter(policy));
}).ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true; // ручная валидация в контроллерах
});

builder.Services.AddTransient<JwtGenerator>();

builder.Services.AddScoped<IInstructionsService, InstructionsService>();
builder.Services.AddScoped<IInstructionStatusService, InstructionStatusService>();
builder.Services.AddScoped<IInstructionsRepository, InstructionsRepository>();
// builder.Services.AddScoped<IInstructionsTreeRepository, InstructionsTreeRepositoryCte>();
// builder.Services.AddScoped<IInstructionsTreeRepository, InstructionsTreeRepositoryPath>();
builder.Services.AddScoped<IInstructionsTreeRepository, InstructionsTreeRepositoryClosure>();
builder.Services.AddScoped<InstructionValidationService>();

builder.Services.AddScoped<RolesService>();
builder.Services.AddScoped<RoleValidationService>();
builder.Services.AddScoped<IRoleRepo, RoleRepo>();

builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<TokensService>();
builder.Services.AddScoped<IUserValidationService, UserValidationService>();
builder.Services.AddScoped<IUserManagerService, UserManagerService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}").RequireAuthorization();

app.UseExceptionHandler(app.Environment.IsDevelopment() ? "/error-development" : "/error");

app.Run();

public partial class Program { }