using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcologyLK.Api.Data;
using EcologyLK.Api.DTOs;
using EcologyLK.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcologyLK.Api.Controllers;

/// <summary>
/// API контроллер для управления Справочником НПА (Нормативно-правовые акты)
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize] // Все методы требуют аутентификации
public class LegalActsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public LegalActsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// GET: api/LegalActs
    /// Получает список всех НПА.
    /// (Доступно всем аутентифицированным пользователям)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LegalActDto>>> GetLegalActs()
    {
        return await _context
            .LegalActs.OrderBy(la => la.ReferenceCode)
            .ProjectTo<LegalActDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    /// <summary>
    /// POST: api/LegalActs
    /// Создает новый НПА.
    /// (Только для Администраторов)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LegalActDto>> CreateLegalAct(
        [FromBody] CreateOrUpdateLegalActDto createDto
    )
    {
        var legalAct = _mapper.Map<LegalAct>(createDto);

        await _context.LegalActs.AddAsync(legalAct);
        await _context.SaveChangesAsync();

        var legalActDto = _mapper.Map<LegalActDto>(legalAct);

        return CreatedAtAction(nameof(GetLegalActs), new { id = legalAct.Id }, legalActDto);
    }

    /// <summary>
    /// PUT: api/LegalActs/{id}
    /// Обновляет существующий НПА.
    /// (Только для Администраторов)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateLegalAct(
        int id,
        [FromBody] CreateOrUpdateLegalActDto updateDto
    )
    {
        var legalAct = await _context.LegalActs.FindAsync(id);
        if (legalAct == null)
        {
            return NotFound("НПА не найден.");
        }

        // Применяем изменения из DTO к существующей модели
        _mapper.Map(updateDto, legalAct);
        _context.Entry(legalAct).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.LegalActs.Any(e => e.Id == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent(); // 204 No Content
    }

    /// <summary>
    /// DELETE: api/LegalActs/{id}
    /// Удаляет НПА.
    /// (Только для Администраторов)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLegalAct(int id)
    {
        var legalAct = await _context.LegalActs.FindAsync(id);
        if (legalAct == null)
        {
            return NotFound("НПА не найден.");
        }

        _context.LegalActs.Remove(legalAct);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
