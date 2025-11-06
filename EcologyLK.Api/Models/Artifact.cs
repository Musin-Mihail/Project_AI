using System.ComponentModel.DataAnnotations; // <-- Добавлено ИИ для исправления ошибки CS0246

namespace EcologyLK.Api.Models;

// Артефакт (документ в хранилище)
public class Artifact
{
    public int Id { get; set; }

    [StringLength(255)]
    public string OriginalFileName { get; set; } = string.Empty; // Оригинальное имя файла (напр. "Проект ПДВ.pdf")

    [StringLength(255)]
    public string StoredFileName { get; set; } = string.Empty; // Имя файла в хранилище (напр. "guid.pdf")

    [StringLength(100)]
    public string MimeType { get; set; } = string.Empty; // Тип контента (напр. "application/pdf")

    public long FileSize { get; set; } // Размер файла в байтах

    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    // Связь: Принадлежит одной площадке
    public int ClientSiteId { get; set; }
    public ClientSite? ClientSite { get; set; }

    // (Опционально) Связь с конкретным требованием
    public int? EcologicalRequirementId { get; set; }
    public EcologicalRequirement? EcologicalRequirement { get; set; }
}
