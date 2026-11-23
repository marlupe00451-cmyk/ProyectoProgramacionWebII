using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEnd.Modules.Auth.Entities;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Modules.Auth.Services;

public class JwtService(IConfiguration configuration) {
  private readonly IConfiguration _config = configuration;

  public string GenerateToken(User user) {
    var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key no configurada.");
    var jwtIssuer = _config["Jwt:Issuer"] ?? "bookcase-api";
    var jwtAudience = _config["Jwt:Audience"] ?? "bookcase-frontend";

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[] {
      new Claim(JwtRegisteredClaimNames.Sub, user.Uuid.ToString()),
      new Claim(JwtRegisteredClaimNames.Email, user.Email),
      new Claim(JwtRegisteredClaimNames.Name, user.Name),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

    var token = new JwtSecurityToken(
      issuer: jwtIssuer,
      audience: jwtAudience,
      claims: claims,
      expires: DateTime.UtcNow.AddHours(8),
      signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
