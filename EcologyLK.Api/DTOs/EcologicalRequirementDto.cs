using EcologyLK.Api.Models;

namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для отображения Экологического Требования
/// </summary>
public class EcologicalRequirementDto
{
    /// <summary>
    /// Уникальный ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название требования (напр. "ПДВ/НДВ").
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Основание (напр. "КоАП РФ 8.21 ч.1").
    /// </summary>
    public string Basis { get; set; } = string.Empty;

    /// <summary>
    /// Описание риска (напр. "Штраф 100-200 тыс.").
    /// </summary>
    public string? PenaltyRisk { get; set; }

    /// <summary>
    /// Ответственный (ФИО).
    /// </summary>
    public string? ResponsiblePerson { get; set; }

    /// <summary>
    /// Срок выполнения.
    /// </summary>
    public DateTime? Deadline { get; set; }

    /// <summary>
    /// Текущий статус выполнения.
    /// </summary>
    public RequirementStatus Status { get; set; }
}
