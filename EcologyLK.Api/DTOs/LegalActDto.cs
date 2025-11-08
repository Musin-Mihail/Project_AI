using System.ComponentModel.DataAnnotations;

namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для отображения НПА
/// </summary>
public class LegalActDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ReferenceCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ExternalLink { get; set; }
}

/// <summary>
/// DTO для создания/обновления НПА
/// </summary>
public class CreateOrUpdateLegalActDto
{
    [Required]
    [StringLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ReferenceCode { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(500)]
    [Url] // Простая валидация URL
    public string? ExternalLink { get; set; }
}
