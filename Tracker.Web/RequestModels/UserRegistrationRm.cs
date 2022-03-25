namespace Tracker.Web.RequestModels;

public class UserRegistrationRm
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? BossId { get; set; }
}