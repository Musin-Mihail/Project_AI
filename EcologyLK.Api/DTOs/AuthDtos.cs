using System.ComponentModel.DataAnnotations;

namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для регистрации нового пользователя
/// </summary>
public class RegisterUserDto
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
}

/// <summary>
/// DTO для входа пользователя
/// </summary>
public class LoginUserDto
{
    /// <summary>
    /// Email.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль.
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO ответа после успешной аутентификации
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// Уникальный ID пользователя.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// ФИО.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// JWT (Bearer token).
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Список ролей (напр. "Admin", "Client").
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// ID привязанного юрлица (Client).
    /// </summary>
    public int? ClientId { get; set; }
}
