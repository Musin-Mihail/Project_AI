using System.ComponentModel.DataAnnotations;

namespace EcologyLK.Api.DTOs;

// --- DTO для управления Клиентами (ЮрЛицами) ---

/// <summary>
/// DTO для отображения информации о Клиенте
/// </summary>
public class ClientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Inn { get; set; } = string.Empty;
    public string Ogrn { get; set; } = string.Empty;
}

/// <summary>
/// DTO для создания нового Клиента
/// </summary>
public class CreateClientDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(12)]
    public string Inn { get; set; } = string.Empty;

    [StringLength(15)]
    public string Ogrn { get; set; } = string.Empty;
}

// --- DTO для управления Пользователями (AppUser) ---

/// <summary>
/// DTO для отображения информации о Пользователе
/// </summary>
public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int? ClientId { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}

/// <summary>
/// DTO для создания нового Пользователя (Администратором)
/// </summary>
public class CreateUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    public int? ClientId { get; set; }
    public string Role { get; set; } = "Client"; // Роль по умолчанию
}

/// <summary>
/// DTO для обновления данных Пользователя
/// </summary>
public class UpdateUserDto
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    public int? ClientId { get; set; }
}
