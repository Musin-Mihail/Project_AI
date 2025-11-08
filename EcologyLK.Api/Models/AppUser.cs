using Microsoft.AspNetCore.Identity;

namespace EcologyLK.Api.Models;

/// <summary>
/// Пользователь системы (Админ, Менеджер, Клиент).
/// Расширяет стандартный IdentityUser.
/// </summary>
public class AppUser : IdentityUser
{
    /// <summary>
    /// Полное имя пользователя (ФИО).
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// (Опционально) ID клиента (юрлица), к которому привязан пользователь.
    /// </summary>
    public int? ClientId { get; set; }

    /// <summary>
    /// Навигационное свойство для Client.
    /// </summary>
    public Client? Client { get; set; }
}
