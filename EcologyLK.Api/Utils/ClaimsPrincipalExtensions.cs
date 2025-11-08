using System.Security.Claims;

namespace EcologyLK.Api.Utils;

/// <summary>
/// Вспомогательный класс (extensions) для удобного получения
/// типизированных данных из Claims (JWT)
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Получает ID пользователя (sub)
    /// </summary>
    /// <param name="user">Объект ClaimsPrincipal (User)</param>
    /// <returns>GUID пользователя в виде строки или null</returns>
    public static string? GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Проверяет, является ли пользователь Admin
    /// </summary>
    /// <param name="user">Объект ClaimsPrincipal (User)</param>
    /// <returns>True, если пользователь имеет роль "Admin"</returns>
    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole("Admin");
    }

    /// <summary>
    /// Получает ID привязанного Клиента (ЮрЛица)
    /// </summary>
    /// <param name="user">Объект ClaimsPrincipal (User)</param>
    /// <returns>ID клиента (int) или null, если не привязан</returns>
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
