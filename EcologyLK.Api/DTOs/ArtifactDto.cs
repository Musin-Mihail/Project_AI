namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для отображения информации об Артефакте (файле)
/// </summary>
public class ArtifactDto
{
    public int Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadDate { get; set; }

    // ID связанных сущностей
    public int ClientSiteId { get; set; }
    public int? EcologicalRequirementId { get; set; }
}
