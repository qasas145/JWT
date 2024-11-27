
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;

public class TokenService : ITokenService
{
    private readonly JWTOptions _options;
    private readonly UserManager<ApplicationUser> _userManager;
    public TokenService(IOptions<JWTOptions> options, UserManager<ApplicationUser> userManager) {
        _options = options.Value;
        _userManager = userManager;
    }
    public async Task<string> GenerateToken(ApplicationUser user)
    {
        // getting roles
        var roles = await _userManager.GetRolesAsync(user);
        var claims = roles.Select(r=>new Claim(ClaimTypes.Role, r));
        claims.Append(new Claim(ClaimTypes.NameIdentifier, user.Id));
        claims.Append(new Claim(ClaimTypes.Name, user.Email));

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor() {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = DateTime.Now.AddDays(_options.ExpireinDays),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)), SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(
                claims.ToArray()
            ),
        };
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);
        return accessToken;
    }
}
