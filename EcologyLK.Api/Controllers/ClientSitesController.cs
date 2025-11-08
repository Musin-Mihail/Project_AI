using AutoMapper;
using EcologyLK.Api.Data;
using EcologyLK.Api.DTOs;
using EcologyLK.Api.Models;
using EcologyLK.Api.Services;
using EcologyLK.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcologyLK.Api.Controllers;

/// <summary>
/// API контроллер для управления Площадками Клиентов (ClientSite)
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class ClientSitesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IRequirementGenerationService _requirementService;

    /// <summary>
    /// Конструктор ClientSitesController
    /// </summary>
    public ClientSitesController(
        AppDbContext context,
        IMapper mapper,
        IRequirementGenerationService requirementService
    )
    {
        _context = context;
        _mapper = mapper;
        _requirementService = requirementService;
    }

    /// <summary>
    /// POST: api/ClientSites
    /// Создает новую площадку клиента ("Анкета")
    /// и автоматически генерирует для нее "Карту требований".
    /// </summary>
    /// <param name="createDto">DTO "Анкеты"</param>
    /// <returns>Созданный DTO площадки</returns>
    /// <response code="201">Возвращает созданную площадку</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Доступ запрещен (попытка создать площадку для другого ClientId)</response>
    /// <response code="404">Указанный ClientId не найден</response>
    [HttpPost]
    [ProducesResponseType(typeof(ClientSiteDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateClientSite([FromBody] CreateClientSiteDto createDto)
    {
        var userClientId = User.GetClientId();
        var isAdmin = User.IsAdmin();

        if (!isAdmin && userClientId != createDto.ClientId)
        {
            return Forbid("Вы не можете создавать площадки для другого клиента.");
        }
        var client = await _context.Clients.FindAsync(createDto.ClientId);
        if (client == null)
        {
            return NotFound($"Client with Id {createDto.ClientId} not found.");
        }

        var site = _mapper.Map<ClientSite>(createDto);

        var requirements = _requirementService.GenerateRequirements(
            site.NvosCategory,
            site.WaterUseType,
            site.HasByproducts
        );

        site.Requirements = requirements;

        await _context.ClientSites.AddAsync(site);
        await _context.SaveChangesAsync();

        var siteDto = _mapper.Map<ClientSiteDto>(site);

        return CreatedAtAction(nameof(GetClientSiteById), new { id = site.Id }, siteDto);
    }

    /// <summary>
    /// GET: api/ClientSites/{id}
    /// Получает площадку клиента по ID, включая "Карту требований".
    /// </summary>
    /// <param name="id">ID площадки</param>
    /// <returns>DTO площадки</returns>
    /// <response code="200">Возвращает DTO площадки</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Доступ к данной площадке запрещен (RLS)</response>
    /// <response code="404">Площадка не найдена</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClientSiteDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ClientSiteDto>> GetClientSiteById(int id)
    {
        var site = await _context
            .ClientSites.Include(s => s.Requirements)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (site == null)
        {
            return NotFound();
        }

        var userClientId = User.GetClientId();
        var isAdmin = User.IsAdmin();

        if (!isAdmin && userClientId != site.ClientId)
        {
            return Forbid("Доступ к данной площадке запрещен.");
        }

        var siteDto = _mapper.Map<ClientSiteDto>(site);
        return Ok(siteDto);
    }
}
