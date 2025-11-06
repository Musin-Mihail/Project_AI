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
    [HttpGet("{id}")]
    public async Task<ActionResult<ClientSiteDto>> GetClientSiteById(int id)
    {
        var site = await _context
            .ClientSites.Include(s => s.Requirements)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (site == null)
        {
            return NotFound();
        }

        var siteDto = _mapper.Map<ClientSiteDto>(site);
        return Ok(siteDto);
    }
}
