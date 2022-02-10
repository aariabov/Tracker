using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Db;
using Tracker.Web.Domain;

namespace Tracker.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrgStructController : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<OrgStructElement>> Get()
    {
        await using var db = new AppDbContext();
        return await db.OrgStruct.ToListAsync();
    }
    
    [HttpPost]
    public async Task<OrgStructElement> CreateOrgStructElement([FromBody]OrgStructElement newOrgStructElement)
    {
        await using var db = new AppDbContext();
        db.OrgStruct.Add(newOrgStructElement);
        await db.SaveChangesAsync();
        return newOrgStructElement;
    }
}