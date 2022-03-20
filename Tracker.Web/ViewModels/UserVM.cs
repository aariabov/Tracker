using Tracker.Web.Domain;

namespace Tracker.Web.ViewModels;

public class UserVm
{
    public string Id { get; }
    public string Token { get; }
    public string Email { get; }
    public string Name { get; }

    public UserVm(string token, User user)
    {
        Id = user.Id;
        Token = token;
        Email = user.Email;
        Name = user.UserName;
    }
}