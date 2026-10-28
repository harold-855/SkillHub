using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SkillHub.Application.Interfaces.Authentication;

namespace SkillHub.Infrastructure.Identity;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<(string AccessToken, DateTime ExpiresAt)> GenerateTokenAsync(
        Guid userId,
        string email,
        IEnumerable<string> roles)
    {
        var jwtSettings = _configuration.GetSection("Jwt");

        var issuer = jwtSettings["Issuer"]
            ?? throw new InvalidOperationException(
                "The JWT issuer is not configured.");

        var audience = jwtSettings["Audience"]
            ?? throw new InvalidOperationException(
                "The JWT audience is not configured.");

        var secretKey = jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException(
                "The JWT secret key is not configured.");

        var expirationMinutes = jwtSettings.GetValue<int>(
            "ExpirationMinutes");

        if (expirationMinutes <= 0)
        {
            throw new InvalidOperationException(
                "JWT expiration minutes must be greater than zero.");
        }

        var keyBytes = Encoding.UTF8.GetBytes(secretKey);

        if (keyBytes.Length < 32)
        {
            throw new InvalidOperationException(
                "The JWT secret key must contain at least 32 bytes.");
        }

        var signingKey = new SymmetricSecurityKey(keyBytes);

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles.Distinct())
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return Task.FromResult((accessToken, expiresAt));
    }
}
