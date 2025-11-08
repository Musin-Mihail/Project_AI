using AutoMapper;
using EcologyLK.Api.Data;
using EcologyLK.Api.DTOs;
using EcologyLK.Api.Models;
using EcologyLK.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcologyLK.Api.Controllers;

/// <summary>
/// API контроллер для управления Экологическими Требованиями
/// (например, ручное редактирование)
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class EcologicalRequirementsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор EcologicalRequirementsController
    /// </summary>
    public EcologicalRequirementsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// PUT: api/EcologicalRequirements/{id}
    /// Обновляет существующее требование (Статус, Срок, Ответственный).
    /// Доступно только Администраторам.
    /// </summary>
    /// <param name="id">ID требования для обновления</param>
    /// <param name="updateDto">DTO с данными для обновления</param>
    /// <returns>204 No Content</returns>
    /// <response code="204">Требование успешно обновлено</response>
    /// <response code="400">Ошибка валидации (не используется, но зарезервировано)</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    /// <response code="403">Пользователь не является Администратором</response>
    /// <response code="404">Требование или связанная площадка не найдены</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")] // Только Админ может менять требования
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRequirement(
        int id,
        [FromBody] UpdateRequirementDto updateDto
    )
    {
        var requirement = await _context.EcologicalRequirements.FindAsync(id);

        if (requirement == null)
        {
            return NotFound("Требование не найдено.");
        }

        // RLS-проверка (на случай, если сюда получат доступ не-Админы)
        var site = await _context.ClientSites.FindAsync(requirement.ClientSiteId);
        if (site == null)
        {
            return NotFound("Связанная площадка не найдена.");
        }

        if (!User.IsAdmin())
        {
            var userClientId = User.GetClientId();
            if (site.ClientId != userClientId)
            {
                return Forbid("Доступ к данному ресурсу запрещен.");
            }
        }

        // Обновляем поля
        requirement.Status = updateDto.Status;
        requirement.Deadline = updateDto.Deadline;
        requirement.ResponsiblePerson = updateDto.ResponsiblePerson;

        _context.Entry(requirement).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.EcologicalRequirements.Any(e => e.Id == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent(); // 204 No Content - стандартный ответ для PUT
    }
}
