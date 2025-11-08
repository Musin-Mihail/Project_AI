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
[Produces("application/json")]
public class RequirementRulesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор RequirementRulesController
    /// </summary>
    public RequirementRulesController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// GET: api/RequirementRules
    /// Получает список всех правил генерации.
    /// </summary>
    /// <returns>Список DTO Правил</returns>
    /// <response code="200">Возвращает список правил</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Пользователь не является Администратором</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RequirementRuleDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
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
    /// <param name="createDto">DTO для создания правила</param>
    /// <returns>Созданный DTO правила</returns>
    /// <response code="201">Возвращает созданное правило</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Пользователь не является Администратором</response>
    [HttpPost]
    [ProducesResponseType(typeof(RequirementRuleDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
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
    /// <param name="id">ID правила для обновления</param>
    /// <param name="updateDto">DTO с данными для обновления</param>
    /// <returns>204 No Content</returns>
    /// <response code="204">Правило успешно обновлено</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Пользователь не является Администратором</response>
    /// <response code="404">Правило не найдено</response>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
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
    /// <param name="id">ID правила для удаления</param>
    /// <returns>204 No Content</returns>
    /// <response code="204">Правило успешно удалено</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Пользователь не является Администратором</response>
    /// <response code="404">Правило не найдено</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
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
