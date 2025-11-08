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
[Produces("application/json")]
public class LegalActsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор LegalActsController
    /// </summary>
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
    /// <returns>Список DTO НПА</returns>
    /// <response code="200">Возвращает список НПА</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LegalActDto>), 200)]
    [ProducesResponseType(401)]
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
    /// <param name="createDto">DTO для создания НПА</param>
    /// <returns>Созданный DTO НПА</returns>
    /// <response code="201">Возвращает созданный НПА</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Пользователь не является Администратором</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(LegalActDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
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
    /// <param name="id">ID НПА для обновления</param>
    /// <param name="updateDto">DTO с данными для обновления</param>
    /// <returns>204 No Content</returns>
    /// <response code="204">НПА успешно обновлен</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Пользователь не является Администратором</response>
    /// <response code="404">НПА не найден</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
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
    /// <param name="id">ID НПА для удаления</param>
    /// <returns>204 No Content</returns>
    /// <response code="204">НПА успешно удален</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Пользователь не является Администратором</response>
    /// <response code="404">НПА не найден</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
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
