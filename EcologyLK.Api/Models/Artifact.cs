using System.ComponentModel.DataAnnotations;

namespace EcologyLK.Api.Models;

/// <summary>
/// Артефакт (метаданные о физическом файле в хранилище).
/// </summary>
public class Artifact
{
    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Оригинальное имя файла (напр. "Проект ПДВ.pdf").
    /// </summary>
    [StringLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// Имя файла в хранилище (напр. "guid.pdf").
    /// </summary>
    [StringLength(255)]
    public string StoredFileName { get; set; } = string.Empty;

    /// <summary>
    /// Тип контента (напр. "application/pdf").
    /// </summary>
    [StringLength(100)]
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Размер файла в байтах.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Дата загрузки файла (UTC).
    /// </summary>
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ID площадки, к которой принадлежит артефакт.
    /// </summary>
    public int ClientSiteId { get; set; }

    /// <summary>
    /// Навигационное свойство для ClientSite.
    /// </summary>
    public ClientSite? ClientSite { get; set; }

    /// <summary>
    /// (Опционально) ID требования, с которым связан артефакт.
    /// </summary>
    public int? EcologicalRequirementId { get; set; }

    /// <summary>
    /// Навигационное свойство для EcologicalRequirement.
    /// </summary>
    public EcologicalRequirement? EcologicalRequirement { get; set; }
}
