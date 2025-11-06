using EcologyLK.Api.Models;

namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для отображения Экологического Требования
/// </summary>
public class EcologicalRequirementDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Basis { get; set; } = string.Empty;
    public string? ResponsiblePerson { get; set; }
    public DateTime? Deadline { get; set; }
    public RequirementStatus Status { get; set; }
}
