using System.ComponentModel.DataAnnotations;

namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для регистрации нового пользователя
/// </summary>
public class RegisterUserDto
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
}

/// <summary>
/// DTO для входа пользователя
/// </summary>
public class LoginUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO ответа после успешной аутентификации
/// </summary>
public class AuthResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public int? ClientId { get; set; }
}
