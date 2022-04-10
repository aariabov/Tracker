using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Tracker.Db;
using Tracker.Db.Models;
using Tracker.Roles.Validators;
using Tracker.Web.Domain;
using Tracker.Web.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=tracker.db"));
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

builder.Services.AddScoped<ExecDateValidator>();
builder.Services.AddScoped<InstructionValidator>();
builder.Services.AddScoped<UserBaseValidator>();
builder.Services.AddScoped<UserCreationValidator>();
builder.Services.AddScoped<UserUpdatingValidator>();
builder.Services.AddScoped<UserDeletingValidator>();
builder.Services.AddScoped<RoleCreationValidator>();
builder.Services.AddScoped<RoleUpdatingValidator>();
builder.Services.AddScoped<RoleDeletingValidator>();

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
var dataSeeder = serviceScope.ServiceProvider.GetService<DataSeeder>();
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
