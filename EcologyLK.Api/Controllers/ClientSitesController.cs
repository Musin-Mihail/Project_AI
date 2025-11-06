using AutoMapper;
using EcologyLK.Api.Data;
using EcologyLK.Api.DTOs;
using EcologyLK.Api.Models;
using EcologyLK.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcologyLK.Api.Controllers;

/// <summary>
/// API контроллер для управления Площадками Клиентов (ClientSite)
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ClientSitesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IRequirementGenerationService _requirementService;

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
    [HttpPost]
    public async Task<IActionResult> CreateClientSite([FromBody] CreateClientSiteDto createDto)
    {
        // 1. Проверяем, существует ли родительский Клиент
        var client = await _context.Clients.FindAsync(createDto.ClientId);
        if (client == null)
        {
            return NotFound($"Client with Id {createDto.ClientId} not found.");
        }

        // 2. Маппим DTO в нашу модель Entity
        var site = _mapper.Map<ClientSite>(createDto);

        // 3. !!! Генерируем "Карту требований" !!!
        var requirements = _requirementService.GenerateRequirements(
            site.NvosCategory,
            site.WaterUseType,
            site.HasByproducts
        );

        // 4. Присваиваем сгенерированные требования площадке
        site.Requirements = requirements;

        // 5. Добавляем площадку (и связанные с ней требования) в БД
        await _context.ClientSites.AddAsync(site);
        await _context.SaveChangesAsync();

        // 6. Маппим результат в DTO для ответа
        var siteDto = _mapper.Map<ClientSiteDto>(site);

        // 7. Возвращаем 201 Created с DTO
        return CreatedAtAction(nameof(GetClientSiteById), new { id = site.Id }, siteDto);
    }

    /// <summary>
    /// GET: api/ClientSites/{id}
    /// Получает площадку клиента по ID, включая "Карту требований".
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ClientSiteDto>> GetClientSiteById(int id)
    {
        // Ищем площадку, сразу включая связанные требования
        var site = await _context
            .ClientSites.Include(s => s.Requirements)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (site == null)
        {
            return NotFound();
        }

        // Маппим Entity в DTO для ответа
        var siteDto = _mapper.Map<ClientSiteDto>(site);
        return Ok(siteDto);
    }
}
