using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Tracker.Web.Domain;

public class JwtGenerator
{
    private readonly SymmetricSecurityKey _key;
    private readonly int _tokenValidityInMinutes;

    public JwtGenerator(IConfiguration config)
    {
        var section = config.GetSection("Token");
        var key = section.GetValue<string>("TokenKey");
        _tokenValidityInMinutes = section.GetValue<int>("TokenValidityInMinutes");

        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }

    public string CreateToken(string userId, string userEmail, IEnumerable<string> userRoles, bool isUserBoss)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.NameId, userId),
            new (JwtRegisteredClaimNames.Email, userEmail),
            new ("isUserBoss", isUserBoss.ToString().ToLower())
        };
        claims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(_tokenValidityInMinutes),
            SigningCredentials = credentials
        };
        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}