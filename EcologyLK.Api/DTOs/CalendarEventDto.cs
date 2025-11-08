namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для отображения события в календаре
/// </summary>
public class CalendarEventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; } // ДОБАВЛЕНО: для событий с длительностью
    public string EventType { get; set; } = "Requirement"; // Тип события (Требование, Платеж и т.д.)
    public int? RelatedSiteId { get; set; } // ID связанной площадки
    public string? RelatedSiteName { get; set; } // Название площадки
    public string? Color { get; set; } // ДОБАВЛЕНО: для FullCalendar
}
