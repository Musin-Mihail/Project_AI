using System.ComponentModel.DataAnnotations;
using EcologyLK.Api.Models;

namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для отображения Правила генерации
/// </summary>
public class RequirementRuleDto
{
    /// <summary>
    /// Уникальный ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Описание для Админа.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Триггер: Кат. I (null = не применять, true = применять).
    /// </summary>
    public bool? TriggerNvosCategoryI { get; set; }

    /// <summary>
    /// Триггер: Кат. II.
    /// </summary>
    public bool? TriggerNvosCategoryII { get; set; }

    /// <summary>
    /// Триггер: Кат. III.
    /// </summary>
    public bool? TriggerNvosCategoryIII { get; set; }

    /// <summary>
    /// Триггер: Кат. IV.
    /// </summary>
    public bool? TriggerNvosCategoryIV { get; set; }

    /// <summary>
    /// Триггер: Тип водопользования (null = не применять).
    /// </summary>
    public WaterUseType? TriggerWaterUseType { get; set; }

    /// <summary>
    /// Триггер: Наличие побочных продуктов (null = не применять).
    /// </summary>
    public bool? TriggerHasByproducts { get; set; }

    /// <summary>
    /// Результат: Название требования.
    /// </summary>
    public string GeneratedTitle { get; set; } = string.Empty;

    /// <summary>
    /// Результат: Основание (НПА).
    /// </summary>
    public string GeneratedBasis { get; set; } = string.Empty;

    /// <summary>
    /// Результат: Риск/Штраф.
    /// </summary>
    public string? GeneratedPenaltyRisk { get; set; }

    /// <summary>
    /// Активно ли правило.
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO для создания/обновления Правила генерации
/// </summary>
public class CreateOrUpdateRuleDto
{
    /// <summary>
    /// Описание для Админа.
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Триггер: Кат. I (null = не применять, true = применять).
    /// </summary>
    public bool? TriggerNvosCategoryI { get; set; }

    /// <summary>
    /// Триггер: Кат. II.
    /// </summary>
    public bool? TriggerNvosCategoryII { get; set; }

    /// <summary>
    /// Триггер: Кат. III.
    /// </summary>
    public bool? TriggerNvosCategoryIII { get; set; }

    /// <summary>
    /// Триггер: Кат. IV.
    /// </summary>
    public bool? TriggerNvosCategoryIV { get; set; }

    /// <summary>
    /// Триггер: Тип водопользования (null = не применять).
    /// </summary>
    public WaterUseType? TriggerWaterUseType { get; set; }

    /// <summary>
    /// Триггер: Наличие побочных продуктов (null = не применять).
    /// </summary>
    public bool? TriggerHasByproducts { get; set; }

    /// <summary>
    /// Результат: Название требования.
    /// </summary>
    [Required]
    [StringLength(500)]
    public string GeneratedTitle { get; set; } = string.Empty;

    /// <summary>
    /// Результат: Основание (НПА).
    /// </summary>
    [Required]
    [StringLength(500)]
    public string GeneratedBasis { get; set; } = string.Empty;

    /// <summary>
    /// Результат: Риск/Штраф.
    /// </summary>
    [StringLength(500)]
    public string? GeneratedPenaltyRisk { get; set; }

    /// <summary>
    /// Активно ли правило.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
