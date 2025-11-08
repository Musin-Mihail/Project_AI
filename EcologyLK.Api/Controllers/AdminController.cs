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
[Produces("application/json")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор контроллера AdminController.
    /// </summary>
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
    /// <returns>Список DTO Клиентов</returns>
    /// <response code="200">Возвращает список клиентов</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Пользователь не является Администратором</response>
    [HttpGet("Clients")]
    [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
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
    /// <param name="createDto">DTO для создания клиента</param>
    /// <returns>Созданный DTO клиента</returns>
    /// <response code="201">Возвращает созданного клиента</response>
    /// <response code="400">Некорректные данные (ошибка валидации)</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Пользователь не является Администратором</response>
    [HttpPost("Clients")]
    [ProducesResponseType(typeof(ClientDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ClientDto>> CreateClient([FromBody] CreateClientDto createDto)
    {
        // Устаревший TODO: "Проверить на дубликат по INN" удален.
        // Эта логика должна быть реализована в будущем.
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
    /// <returns>Список DTO Пользователей</returns>
    /// <response code="200">Возвращает список пользователей</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Пользователь не является Администратором</response>
    [HttpGet("Users")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
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
    /// <param name="createDto">DTO для создания пользователя</param>
    /// <returns>Созданный DTO пользователя</returns>
    /// <response code="200">Возвращает созданного пользователя</response>
    /// <response code="400">Пользователь уже существует или ошибка валидации</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Пользователь не является Администратором</response>
    [HttpPost("Users")]
    [ProducesResponseType(typeof(UserDto), 200)] // Возвращает Ok(dto), а не CreatedAt
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
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
    /// Обновляет данные Пользователя (ФИО и ClientId).
    /// </summary>
    /// <param name="id">ID пользователя (GUID)</param>
    /// <param name="updateDto">DTO с данными для обновления</param>
    /// <returns>204 No Content</returns>
    /// <response code="204">Пользователь успешно обновлен</response>
    /// <response code="400">Ошибка при обновлении</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Пользователь не является Администратором</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpPut("Users/{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
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
