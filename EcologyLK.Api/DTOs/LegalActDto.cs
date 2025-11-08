using System.ComponentModel.DataAnnotations;

namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для отображения НПА
/// </summary>
public class LegalActDto
{
    /// <summary>
    /// Уникальный ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Полное название.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Код (напр. "ФЗ-7").
    /// </summary>
    public string ReferenceCode { get; set; } = string.Empty;

    /// <summary>
    /// Краткое описание.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Внешняя ссылка (напр. на consultant.ru).
    /// </summary>
    public string? ExternalLink { get; set; }
}

/// <summary>
/// DTO для создания/обновления НПА
/// </summary>
public class CreateOrUpdateLegalActDto
{
    /// <summary>
    /// Полное название.
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Код (напр. "ФЗ-7").
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ReferenceCode { get; set; } = string.Empty;

    /// <summary>
    /// Краткое описание.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Внешняя ссылка (напр. на consultant.ru).
    /// </summary>
    [StringLength(500)]
    [Url]
    public string? ExternalLink { get; set; }
}
