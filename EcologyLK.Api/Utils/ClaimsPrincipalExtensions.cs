using System.Security.Claims;

namespace EcologyLK.Api.Utils;

/// <summary>
/// Вспомогательный класс для удобного получения
/// данных из Claims (JWT)
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Получает ID пользователя (sub)
    /// </summary>
    public static string? GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Проверяет, является ли пользователь Admin
    /// </summary>
    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole("Admin");
    }

    /// <summary>
    /// Получает ID привязанного Клиента (ЮрЛица)
    /// </summary>
    public static int? GetClientId(this ClaimsPrincipal user)
    {
        var clientIdStr = user.FindFirst("client_id")?.Value;
        if (int.TryParse(clientIdStr, out var clientId))
        {
            return clientId;
        }
        return null;
    }
}
