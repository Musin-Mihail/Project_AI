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
/// API контроллер для управления Справочником "Правил генерации требований".
/// (Только для Администраторов)
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class RequirementRulesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public RequirementRulesController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// GET: api/RequirementRules
    /// Получает список всех правил генерации.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RequirementRuleDto>>> GetRequirementRules()
    {
        return await _context
            .RequirementRules.OrderBy(r => r.Id)
            .ProjectTo<RequirementRuleDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    /// <summary>
    /// POST: api/RequirementRules
    /// Создает новое правило.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RequirementRuleDto>> CreateRule(
        [FromBody] CreateOrUpdateRuleDto createDto
    )
    {
        var rule = _mapper.Map<RequirementRule>(createDto);

        await _context.RequirementRules.AddAsync(rule);
        await _context.SaveChangesAsync();

        var ruleDto = _mapper.Map<RequirementRuleDto>(rule);

        return CreatedAtAction(nameof(GetRequirementRules), new { id = rule.Id }, ruleDto);
    }

    /// <summary>
    /// PUT: api/RequirementRules/{id}
    /// Обновляет существующее правило.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRule(int id, [FromBody] CreateOrUpdateRuleDto updateDto)
    {
        var rule = await _context.RequirementRules.FindAsync(id);
        if (rule == null)
        {
            return NotFound("Правило не найдено.");
        }

        _mapper.Map(updateDto, rule);
        _context.Entry(rule).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.RequirementRules.Any(e => e.Id == id))
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
    /// DELETE: api/RequirementRules/{id}
    /// Удаляет правило.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRule(int id)
    {
        var rule = await _context.RequirementRules.FindAsync(id);
        if (rule == null)
        {
            return NotFound("Правило не найдено.");
        }

        _context.RequirementRules.Remove(rule);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
