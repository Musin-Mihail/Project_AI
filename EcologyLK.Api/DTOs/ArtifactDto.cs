namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для отображения информации об Артефакте (файле)
/// </summary>
public class ArtifactDto
{
    /// <summary>
    /// Уникальный ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Оригинальное имя файла (напр. "Проект ПДВ.pdf").
    /// </summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// Тип контента (напр. "application/pdf").
    /// </summary>
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Размер файла в байтах.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Дата загрузки файла (UTC).
    /// </summary>
    public DateTime UploadDate { get; set; }

    /// <summary>
    /// ID площадки, к которой принадлежит артефакт.
    /// </summary>
    public int ClientSiteId { get; set; }

    /// <summary>
    /// (Опционально) ID требования, с которым связан артефакт.
    /// </summary>
    public int? EcologicalRequirementId { get; set; }
}
