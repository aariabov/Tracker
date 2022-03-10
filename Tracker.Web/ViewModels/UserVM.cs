namespace Tracker.Web.ViewModels;

public class UserVm
{
    public string Token { get; }
    public string Email { get; }

    public UserVm(string token, string email)
    {
        Token = token;
        Email = email;
    }
}