namespace Tracker.Users.RequestModels;

public class UserBaseRm
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? BossId { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
