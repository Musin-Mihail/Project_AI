using System.ComponentModel.DataAnnotations;

namespace EcologyLK.Api.DTOs;

// --- DTO для управления Клиентами (ЮрЛицами) ---

/// <summary>
/// DTO для отображения информации о Клиенте
/// </summary>
public class ClientDto
{
    /// <summary>
    /// Уникальный ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Наименование организации.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ИНН.
    /// </summary>
    public string Inn { get; set; } = string.Empty;

    /// <summary>
    /// ОГРН.
    /// </summary>
    public string Ogrn { get; set; } = string.Empty;
}

/// <summary>
/// DTO для создания нового Клиента
/// </summary>
public class CreateClientDto
{
    /// <summary>
    /// Наименование организации.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ИНН.
    /// </summary>
    [Required]
    [StringLength(12)]
    public string Inn { get; set; } = string.Empty;

    /// <summary>
    /// ОГРН.
    /// </summary>
    [StringLength(15)]
    public string Ogrn { get; set; } = string.Empty;
}

// --- DTO для управления Пользователями (AppUser) ---

/// <summary>
/// DTO для отображения информации о Пользователе
/// </summary>
public class UserDto
{
    /// <summary>
    /// Уникальный ID (Guid).
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// ФИО.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// ID привязанного юрлица (Client).
    /// </summary>
    public int? ClientId { get; set; }

    /// <summary>
    /// Список ролей (напр. "Admin", "Client").
    /// </summary>
    public IList<string> Roles { get; set; } = new List<string>();
}

/// <summary>
/// DTO для создания нового Пользователя (Администратором)
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// Email (будет логином).
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// ФИО.
    /// </summary>
    [Required]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// ID привязанного юрлица (Client).
    /// </summary>
    public int? ClientId { get; set; }

    /// <summary>
    /// Назначаемая роль (по умолч. "Client").
    /// </summary>
    public string Role { get; set; } = "Client";
}

/// <summary>
/// DTO для обновления данных Пользователя
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// ФИО.
    /// </summary>
    [Required]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// ID привязанного юрлица (Client).
    /// </summary>
    public int? ClientId { get; set; }
}
