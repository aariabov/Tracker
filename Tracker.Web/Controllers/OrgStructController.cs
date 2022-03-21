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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrgStructElementVm>))]
    public async Task<ActionResult<IEnumerable<OrgStructElementVm>>> Get()
    {
        var users = await _userManager.Users
            .Select(u => new OrgStructElementVm(u.Id, u.UserName, u.BossId))
            .ToArrayAsync();
        
        return Ok(users);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrgStructElement))]
    public async Task<ActionResult<OrgStructElement>> CreateOrgStructElement([FromBody]OrgStructElement newOrgStructElement)
    {
        // TODO: добавить валидацию
        _db.OrgStruct.Add(newOrgStructElement);
        await _db.SaveChangesAsync();
        return Ok(newOrgStructElement);
    }
}