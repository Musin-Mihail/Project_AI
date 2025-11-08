using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcologyLK.Api.Data;
using EcologyLK.Api.DTOs;
using EcologyLK.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcologyLK.Api.Controllers;

/// <summary>
/// API контроллер для управления Финансовыми документами (Договора, Счета, Акты)
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class FinancialDocumentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор FinancialDocumentsController
    /// </summary>
    public FinancialDocumentsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// (Приватный) Проверка, имеет ли пользователь (Клиент или Админ)
    /// доступ к указанной площадке (RLS).
    /// </summary>
    private async Task<bool> CheckSiteAccessAsync(int siteId)
    {
        if (User.IsAdmin())
            return true;

        var userClientId = User.GetClientId();
        if (!userClientId.HasValue)
            return false; // Пользователь не привязан к клиенту

        var site = await _context.ClientSites.FindAsync(siteId);
        return site != null && site.ClientId == userClientId.Value;
    }

    /// <summary>
    /// GET: api/FinancialDocuments?siteId=5
    /// Получает список финансовых документов для указанной площадки.
    /// </summary>
    /// <param name="siteId">ID площадки, для которой запрашиваются документы</param>
    /// <returns>Список DTO Финансовых документов</returns>
    /// <response code="200">Возвращает список документов</response>
    /// <response code="400">Не указан siteId</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Доступ к данной площадке запрещен (RLS)</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FinancialDocumentDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IEnumerable<FinancialDocumentDto>>> GetDocumentsForSite(
        [FromQuery] int siteId
    )
    {
        if (siteId <= 0)
        {
            return BadRequest("Необходимо указать siteId.");
        }
        if (!await CheckSiteAccessAsync(siteId))
        {
            return Forbid("Доступ к данной площадке запрещен.");
        }

        var documents = await _context
            .FinancialDocuments.Where(d => d.ClientSiteId == siteId)
            .OrderByDescending(d => d.DocumentDate)
            .ProjectTo<FinancialDocumentDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Ok(documents);
    }
}
