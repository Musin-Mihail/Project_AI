namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для отображения события в календаре
/// </summary>
public class CalendarEventDto
{
    /// <summary>
    /// ID события (ID Требования).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название события (Название Требования).
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Дата начала (Deadline).
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Дата окончания (для событий с длительностью).
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Тип события (напр. "Requirement").
    /// </summary>
    public string EventType { get; set; } = "Requirement";

    /// <summary>
    /// ID связанной площадки.
    /// </summary>
    public int? RelatedSiteId { get; set; }

    /// <summary>
    /// Название связанной площадки.
    /// </summary>
    public string? RelatedSiteName { get; set; }

    /// <summary>
    /// Цвет события для FullCalendar.
    /// </summary>
    public string? Color { get; set; }
}
