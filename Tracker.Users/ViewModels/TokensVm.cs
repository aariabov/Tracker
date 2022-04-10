namespace Tracker.Users.ViewModels;

public class TokensVm
{
    public string Token { get; }
    public string RefreshToken { get; }

    public TokensVm(string token, string refreshToken)
    {
        Token = token;
        RefreshToken = refreshToken;
    }
}