using System.ComponentModel.DataAnnotations;
using EcologyLK.Api.Models;

namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для ручного обновления Экологического Требования (Администратором)
/// </summary>
public class UpdateRequirementDto
{
    /// <summary>
    /// Новый статус.
    /// </summary>
    [Required]
    public RequirementStatus Status { get; set; }

    /// <summary>
    /// Новый срок выполнения.
    /// </summary>
    public DateTime? Deadline { get; set; }

    /// <summary>
    /// Новый ответственный.
    /// </summary>
    [StringLength(200)]
    public string? ResponsiblePerson { get; set; }
}
