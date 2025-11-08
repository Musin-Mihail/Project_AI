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
public class EcologicalRequirementsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

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
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")] // Только Админ может менять требования
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

        // RLS-проверка (хотя [Authorize(Roles="Admin")] уже это покрывает,
        // но это хорошая практика на случай расширения ролей)
        var site = await _context.ClientSites.FindAsync(requirement.ClientSiteId);
        if (site == null)
        {
            return NotFound("Связанная площадка не найдена.");
        }

        if (!User.IsAdmin())
        {
            // Эта проверка сработает, если убрать [Authorize(Roles="Admin")]
            // и добавить роль "Manager"
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
