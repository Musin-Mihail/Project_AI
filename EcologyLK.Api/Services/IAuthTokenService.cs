using EcologyLK.Api.Models;

namespace EcologyLK.Api.Services;

/// <summary>
/// Интерфейс сервиса для генерации JWT (JSON Web Tokens)
/// </summary>
public interface IAuthTokenService
{
    /// <summary>
    /// Генерирует JWT для указанного пользователя
    /// </summary>
    /// <param name="user">Пользователь (AppUser)</param>
    /// <param name="roles">Список ролей пользователя</param>
    /// <returns>Строка с JWT</returns>
    string GenerateToken(AppUser user, IEnumerable<string> roles);
}
