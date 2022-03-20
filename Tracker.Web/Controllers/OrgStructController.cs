using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Db;
using Tracker.Web.Domain;
using Tracker.Web.ViewModels;

namespace Tracker.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrgStructController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;

    public OrgStructController(AppDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IEnumerable<OrgStructElementVm>> Get()
    {
        var users = await _userManager.Users
            .Select(u => new OrgStructElementVm(u.Id, u.UserName, u.BossId))
            .ToArrayAsync();
        
        return users;
    }
    
    [HttpPost]
    public async Task<OrgStructElement> CreateOrgStructElement([FromBody]OrgStructElement newOrgStructElement)
    {
        _db.OrgStruct.Add(newOrgStructElement);
        await _db.SaveChangesAsync();
        return newOrgStructElement;
    }
}