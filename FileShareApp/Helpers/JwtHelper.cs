using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FileShareApp.Backend.Helpers;

public class JwtHelper
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _signingKey;

    public JwtHelper(IConfiguration configuration)
    {
        _configuration = configuration;
        var secret = _configuration["Jwt:Secret"]
            ?? throw new ArgumentNullException("Jwt:Secret is missing in config");
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }

    public string GenerateToken(string username, string? role = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.NameIdentifier, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrEmpty(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configuration["Jwt:Issuer"] ?? "FileShareApp",
            Audience = _configuration["Jwt:Audience"] ?? "FileShareAppUsers",
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(
                _configuration.GetValue<double>("Jwt:ExpireHours", 1)
            ),
            SigningCredentials = new SigningCredentials(
                _signingKey,
                SecurityAlgorithms.HmacSha256
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}