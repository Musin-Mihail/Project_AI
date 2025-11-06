namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для отображения полной информации о площадке (включая ее "Карту требований")
/// </summary>
public class ClientSiteDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // Вложенный список DTO требований
    public List<EcologicalRequirementDto> Requirements { get; set; } = new();
}
