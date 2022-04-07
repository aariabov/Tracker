using Tracker.Web.Domain;

namespace Tracker.Web.ViewModels;

public class OrgStructElementVm
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? ParentId { get; set; }
    public IEnumerable<string> Roles { get; set; }
}