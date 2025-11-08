using System.ComponentModel.DataAnnotations;
using EcologyLK.Api.Models;

namespace EcologyLK.Api.Models;

/// <summary>
/// Модель Справочника: Правило для генерации Экологического Требования.
/// (Заменит hardcode-логику в RequirementGenerationService)
/// </summary>
public class RequirementRule
{
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty; // Описание для Админа (напр. "Требование ПДВ для I-II кат.")

    // --- Условия (Триггеры) ---

    // Если null - не применять фильтр
    public bool? TriggerNvosCategoryI { get; set; }
    public bool? TriggerNvosCategoryII { get; set; }
    public bool? TriggerNvosCategoryIII { get; set; }
    public bool? TriggerNvosCategoryIV { get; set; }

    // Если null - не применять фильтр
    public WaterUseType? TriggerWaterUseType { get; set; }

    // Если null - не применять фильтр
    public bool? TriggerHasByproducts { get; set; }

    // --- Результат (Что создать) ---
    [Required]
    [StringLength(500)]
    public string GeneratedTitle { get; set; } = string.Empty; // Название требования (напр. "ПДВ/НДВ")

    [Required]
    [StringLength(500)]
    public string GeneratedBasis { get; set; } = string.Empty; // Основание (напр. "КоАП РФ 8.21 ч.1")

    [StringLength(500)]
    public string? GeneratedPenaltyRisk { get; set; } // Описание риска (напр. "Штраф 100-200 тыс.")

    public bool IsActive { get; set; } = true; // Можно временно отключать правило
}
