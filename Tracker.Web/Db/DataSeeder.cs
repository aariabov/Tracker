using Microsoft.AspNetCore.Identity;
using Tracker.Web.Domain;

namespace Tracker.Web.Db;

public class DataSeeder
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IConfiguration _config;

    public DataSeeder(UserManager<User> userManager
        , RoleManager<Role> roleManager
        , IConfiguration config)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
    }

    public async Task Seed()
    {
        var defaultAdminSection = _config.GetSection("DefaultAdmin");
        var adminName = defaultAdminSection.GetValue<string>("Name");
        var adminEmail = defaultAdminSection.GetValue<string>("Email");
        var adminPassword = defaultAdminSection.GetValue<string>("Password");
        var adminRole = defaultAdminSection.GetValue<string>("Role");

        var isAdminRoleExists = await _roleManager.RoleExistsAsync(adminRole);
        if (!isAdminRoleExists)
        {
            var result = await _roleManager.CreateAsync(new Role(adminRole));
            if (!result.Succeeded)
                throw new Exception($"Error when creation role: {string.Join(", ", result.Errors)}");
        }

        var admin = await _userManager.FindByEmailAsync(adminEmail);
        if(admin is null)
        {
            var adminUser = new User(adminName , adminEmail, bossId: null);
            var createdAdmin = await _userManager.CreateAsync(adminUser, adminPassword);
            if (createdAdmin.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}