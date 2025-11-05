namespace EcologyLK.Api.Models;

// Экологическое требование (из "Карты требований")
public class EcologicalRequirement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty; // Название требования (напр. "ПДВ/НДВ")
    public string Basis { get; set; } = string.Empty; // Основание (напр. "КоАП РФ 8.21 ч.1")
    public string? ResponsiblePerson { get; set; } // Ответственный
    public DateTime? Deadline { get; set; } // Срок выполнения
    public RequirementStatus Status { get; set; } = RequirementStatus.NotStarted;

    // Связь: Принадлежит одной площадке
    public int ClientSiteId { get; set; }
    public ClientSite? ClientSite { get; set; }
}
