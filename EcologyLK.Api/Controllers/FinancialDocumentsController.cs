using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcologyLK.Api.Data;
using EcologyLK.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcologyLK.Api.Controllers;

/// <summary>
/// API контроллер для управления Финансовыми документами (Договора, Счета, Акты)
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class FinancialDocumentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public FinancialDocumentsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// GET: api/FinancialDocuments?siteId=5
    /// Получает список финансовых документов для указанной площадки.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FinancialDocumentDto>>> GetDocumentsForSite(
        [FromQuery] int siteId
    )
    {
        if (siteId <= 0)
        {
            return BadRequest("Необходимо указать siteId.");
        }

        var documents = await _context
            .FinancialDocuments.Where(d => d.ClientSiteId == siteId)
            .OrderByDescending(d => d.DocumentDate)
            .ProjectTo<FinancialDocumentDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Ok(documents);
    }

    // TODO: Для MVP реализован только GET.
    // В будущем здесь будут методы [HttpPost] для создания, [HttpPut] для смены статуса
    // и, возможно, привязка к IArtifactStorageService для загрузки сканов.
}
