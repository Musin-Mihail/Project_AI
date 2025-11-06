using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcologyLK.Api.Data;
using EcologyLK.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcologyLK.Api.Controllers;

/// <summary>
/// API контроллер для получения данных "Календаря событий"
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CalendarEventsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CalendarEventsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// GET: api/CalendarEvents
    /// Получает список всех событий (для MVP - это требования с установленным Deadline)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CalendarEventDto>>> GetCalendarEvents()
    {
        // В будущем здесь должна быть фильтрация по ClientId/UserId
        // на основе аутентификации [Authorize]

        var events = await _context
            .EcologicalRequirements
            // Выбираем только те, у которых есть срок выполнения
            .Where(r => r.Deadline.HasValue)
            .OrderBy(r => r.Deadline)
            .Include(r => r.ClientSite) // Включаем данные площадки
            .ProjectTo<CalendarEventDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Ok(events);
    }
}
