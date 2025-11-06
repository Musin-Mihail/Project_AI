using EcologyLK.Api.DTOs;
using EcologyLK.Api.Models;
using EcologyLK.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcologyLK.Api.Controllers;

/// <summary>
/// API контроллер для Аутентификации (Регистрация, Вход)
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IAuthTokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    // Роль по умолчанию для новых регистраций
    private const string DefaultRole = "Client";

    public AuthController(
        UserManager<AppUser> userManager,
        IAuthTokenService tokenService,
        ILogger<AuthController> logger
    )
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// POST: api/Auth/Register
    /// Регистрирует нового пользователя
    /// </summary>
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
    {
        var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
        if (userExists != null)
        {
            return BadRequest(new { message = "Пользователь с таким Email уже существует" });
        }

        var user = new AppUser
        {
            Email = registerDto.Email,
            UserName = registerDto.Email,
            FullName = registerDto.FullName,
            // TODO: Привязать ClientId
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Ошибка регистрации пользователя {Email}: {Errors}",
                registerDto.Email,
                result.Errors.Select(e => e.Description)
            );
            return BadRequest(new { message = "Ошибка при регистрации", errors = result.Errors });
        }

        // Присваиваем роль по умолчанию
        await _userManager.AddToRoleAsync(user, DefaultRole);

        return Ok(
            new
            {
                message = $"Пользователь {user.Email} успешно зарегистрирован в роли {DefaultRole}",
            }
        );
    }

    /// <summary>
    /// POST: api/Auth/Login
    /// Выполняет вход пользователя и возвращает JWT
    /// </summary>
    [HttpPost("Login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginUserDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Неверный Email или пароль" });
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid)
        {
            return Unauthorized(new { message = "Неверный Email или пароль" });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        var response = new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Token = token,
            Roles = roles.ToList(),
        };

        return Ok(response);
    }
}
