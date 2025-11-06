using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcologyLK.Api.Data;
using EcologyLK.Api.DTOs;
using EcologyLK.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcologyLK.Api.Controllers;

/// <summary>
/// API контроллер для "Админ-панели".
/// Предоставляет эндпоинты для управления
/// Клиентами (ЮрЛицами) и Пользователями.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")] // Только Админ имеет доступ
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public AdminController(
        AppDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IMapper mapper
    )
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }

    // --- Управление Клиентами (ЮрЛицами) ---

    /// <summary>
    /// GET: api/Admin/Clients
    /// Получает список всех Клиентов (ЮрЛиц).
    /// </summary>
    [HttpGet("Clients")]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
    {
        return await _context
            .Clients.ProjectTo<ClientDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    /// <summary>
    /// POST: api/Admin/Clients
    /// Создает нового Клиента (ЮрЛицо).
    /// </summary>
    [HttpPost("Clients")]
    public async Task<ActionResult<ClientDto>> CreateClient([FromBody] CreateClientDto createDto)
    {
        // TODO: Проверить на дубликат по INN
        var client = _mapper.Map<Client>(createDto);
        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();

        var clientDto = _mapper.Map<ClientDto>(client);
        return CreatedAtAction(nameof(GetClients), new { id = client.Id }, clientDto);
    }

    // --- Управление Пользователями (AppUser) ---

    /// <summary>
    /// GET: api/Admin/Users
    /// Получает список всех Пользователей.
    /// </summary>
    [HttpGet("Users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            userDtos.Add(
                new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    ClientId = user.ClientId,
                    Roles = await _userManager.GetRolesAsync(user),
                }
            );
        }
        return Ok(userDtos);
    }

    /// <summary>
    /// POST: api/Admin/Users
    /// Создает нового Пользователя (Администратором).
    /// </summary>
    [HttpPost("Users")]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createDto)
    {
        var userExists = await _userManager.FindByEmailAsync(createDto.Email);
        if (userExists != null)
        {
            return BadRequest(new { message = "Пользователь с таким Email уже существует" });
        }

        var user = new AppUser
        {
            Email = createDto.Email,
            UserName = createDto.Email,
            FullName = createDto.FullName,
            ClientId = createDto.ClientId,
        };

        var result = await _userManager.CreateAsync(user, createDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(
                new { message = "Ошибка при создании пользователя", errors = result.Errors }
            );
        }

        // Проверяем, существует ли роль
        if (
            !string.IsNullOrEmpty(createDto.Role)
            && await _roleManager.RoleExistsAsync(createDto.Role)
        )
        {
            await _userManager.AddToRoleAsync(user, createDto.Role);
        }
        else
        {
            // По умолчанию (если роль не указана или не найдена)
            await _userManager.AddToRoleAsync(user, "Client");
        }

        var newUserDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            ClientId = user.ClientId,
            Roles = await _userManager.GetRolesAsync(user),
        };

        return Ok(newUserDto);
    }

    /// <summary>
    /// PUT: api/Admin/Users/{id}
    /// Обновляет данные Пользователя.
    /// </summary>
    [HttpPut("Users/{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("Пользователь не найден.");
        }

        user.FullName = updateDto.FullName;
        user.ClientId = updateDto.ClientId; // Привязка/смена клиента

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(
                new { message = "Ошибка при обновлении пользователя", errors = result.Errors }
            );
        }

        return NoContent();
    }
}
