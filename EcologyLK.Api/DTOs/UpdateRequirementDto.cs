using System.ComponentModel.DataAnnotations;
using EcologyLK.Api.Models;

namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для ручного обновления Экологического Требования (Администратором)
/// </summary>
public class UpdateRequirementDto
{
    [Required]
    public RequirementStatus Status { get; set; }

    public DateTime? Deadline { get; set; }

    [StringLength(200)]
    public string? ResponsiblePerson { get; set; }
}
