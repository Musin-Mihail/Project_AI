namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для отображения полной информации о площадке (включая ее "Карту требований")
/// </summary>
public class ClientSiteDto
{
    /// <summary>
    /// Уникальный ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название площадки.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Адрес площадки.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Вложенный список DTO требований ("Карта требований").
    /// </summary>
    public List<EcologicalRequirementDto> Requirements { get; set; } = new();
}
