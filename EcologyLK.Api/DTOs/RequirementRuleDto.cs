using System.ComponentModel.DataAnnotations;
using EcologyLK.Api.Models;

namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для отображения Правила генерации
/// </summary>
public class RequirementRuleDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool? TriggerNvosCategoryI { get; set; }
    public bool? TriggerNvosCategoryII { get; set; }
    public bool? TriggerNvosCategoryIII { get; set; }
    public bool? TriggerNvosCategoryIV { get; set; }
    public WaterUseType? TriggerWaterUseType { get; set; }
    public bool? TriggerHasByproducts { get; set; }
    public string GeneratedTitle { get; set; } = string.Empty;
    public string GeneratedBasis { get; set; } = string.Empty;
    public string? GeneratedPenaltyRisk { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO для создания/обновления Правила генерации
/// </summary>
public class CreateOrUpdateRuleDto
{
    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public bool? TriggerNvosCategoryI { get; set; }
    public bool? TriggerNvosCategoryII { get; set; }
    public bool? TriggerNvosCategoryIII { get; set; }
    public bool? TriggerNvosCategoryIV { get; set; }

    public WaterUseType? TriggerWaterUseType { get; set; }
    public bool? TriggerHasByproducts { get; set; }

    [Required]
    [StringLength(500)]
    public string GeneratedTitle { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string GeneratedBasis { get; set; } = string.Empty;

    [StringLength(500)]
    public string? GeneratedPenaltyRisk { get; set; }

    public bool IsActive { get; set; } = true;
}
