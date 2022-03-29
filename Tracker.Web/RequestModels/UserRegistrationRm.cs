namespace Tracker.Web.RequestModels;

public class UserRegistrationRm
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? BossId { get; set; }
}