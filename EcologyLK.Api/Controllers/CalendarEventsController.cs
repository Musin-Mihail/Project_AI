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
        var userClientId = User.GetClientId();
        var isAdmin = User.IsAdmin();

        // --- ИСПРАВЛЕНА ЛОГИКА RLS ---
        // 1. Базовый запрос
        var query = _context
            .EcologicalRequirements
            // Включаем ClientSite, чтобы маппер мог получить RelatedSiteName
            .Include(r => r.ClientSite)
            .Where(r => r.Deadline.HasValue); // Выбираем только те, у которых есть срок

        if (!isAdmin)
        {
            if (!userClientId.HasValue)
            {
                // Пользователь не-админ и не привязан к клиенту = не видит ничего
                return Ok(new List<CalendarEventDto>());
            }

            // 2. Получаем ID площадок, к которым у клиента есть доступ
            var clientSiteIds = await _context
                .ClientSites.Where(s => s.ClientId == userClientId.Value)
                .Select(s => s.Id)
                .ToListAsync();

            // 3. Фильтруем требования по этим площадкам (RLS)
            query = query.Where(r => clientSiteIds.Contains(r.ClientSiteId));
        }
        // Админ видит все (фильтр RLS не применяется)

        // 4. Выполняем отфильтрованный запрос
        var events = await query
            .OrderBy(r => r.Deadline)
            .ProjectTo<CalendarEventDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        // --- КОНЕЦ ИСПРАВЛЕНИЯ ---

        return Ok(events);
    }
}
