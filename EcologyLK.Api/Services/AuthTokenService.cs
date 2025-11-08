using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcologyLK.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace EcologyLK.Api.Services;

/// <summary>
/// Реализация сервиса генерации JWT
/// </summary>
public class AuthTokenService : IAuthTokenService
{
    private readonly IConfiguration _config;

    /// <summary>
    /// Конструктор AuthTokenService
    /// </summary>
    /// <param name="config">Конфигурация приложения (для доступа к Jwt:SecretKey)</param>
    public AuthTokenService(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Генерирует JWT для указанного пользователя
    /// </summary>
    /// <param name="user">Пользователь (AppUser)</param>
    /// <param name="roles">Список ролей пользователя</param>
    /// <returns>Строка с JWT</returns>
    public string GenerateToken(AppUser user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (user.ClientId.HasValue)
        {
            claims.Add(new Claim("client_id", user.ClientId.Value.ToString()));
        }

        var jwtKey = _config["Jwt:SecretKey"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT SecretKey не настроен в appsettings.json");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(7); // Срок жизни токена

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
