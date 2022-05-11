namespace Tracker.Roles.RequestModels;

public class RoleUpdatingRm : RoleCreationRm
{
    public string Id { get; set; }
    public string ConcurrencyStamp { get; set; }
}