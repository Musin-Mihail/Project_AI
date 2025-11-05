namespace EcologyLK.Api.Models;

// Артефакт (документ в хранилище)
public class Artifact
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty; // Имя файла (напр. "Проект ПДВ.pdf")
    public string FilePath { get; set; } = string.Empty; // Путь в S3 / Azure Blob / Файловой системе
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    // Связь: Принадлежит одной площадке
    public int ClientSiteId { get; set; }
    public ClientSite? ClientSite { get; set; }

    // (Опционально) Связь с конкретным требованием
    // public int? EcologicalRequirementId { get; set; }
    // public EcologicalRequirement? EcologicalRequirement { get; set; }
}
