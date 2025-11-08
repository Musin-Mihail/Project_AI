using System.ComponentModel.DataAnnotations;

namespace EcologyLK.Api.Models;

/// <summary>
/// Модель Справочника: Нормативно-правовой акт (НПА)
/// </summary>
public class LegalAct
{
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Title { get; set; } = string.Empty; // Напр., "Федеральный закон 'Об охране окружающей среды'"

    [StringLength(100)]
    public string ReferenceCode { get; set; } = string.Empty; // Напр., "ФЗ-7" или "КоАП РФ 8.21 ч.1"

    [StringLength(1000)]
    public string? Description { get; set; } // Краткое описание

    [StringLength(500)]
    public string? ExternalLink { get; set; } // Ссылка на consultant.ru и т.п.
}
