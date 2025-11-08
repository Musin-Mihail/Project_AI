using EcologyLK.Api.Data;
using EcologyLK.Api.Models;

namespace EcologyLK.Api.Services;

/// <summary>
/// Реализация сервиса генерации требований.
/// (РЕФАКТОРИНГ, ЭТАП 22)
/// Логика больше не "hardcoded", а основана на
/// Справочнике RequirementRules в AppDbContext.
/// </summary>
public class RequirementGenerationService : IRequirementGenerationService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Конструктор RequirementGenerationService
    /// </summary>
    /// <param name="context">Контекст БД (для доступа к RequirementRules)</param>
    public RequirementGenerationService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Генерирует список экологических требований на основе
    /// правил из базы данных (Справочник RequirementRules).
    /// </summary>
    /// <param name="category">Категория НВОС</param>
    /// <param name="waterUse">Тип водопользования</param>
    /// <param name="hasByproducts">Наличие побочных продуктов</param>
    /// <returns>Список сгенерированных требований</returns>
    public List<EcologicalRequirement> GenerateRequirements(
        NvosCategory category,
        WaterUseType waterUse,
        bool hasByproducts
    )
    {
        // 1. Получаем все активные правила из БД
        var allRules = _context.RequirementRules.Where(r => r.IsActive).ToList();

        var applicableRules = new List<RequirementRule>();

        // 2. Определяем, какие правила применяются
        foreach (var rule in allRules)
        {
            bool applies = false;

            // 2.1. Проверка по Категории НВОС
            // (Логика основана на том, как DataSeeder заполнил правила:
            // "true" - применять, "null" - не применять)
            if (category == NvosCategory.I && rule.TriggerNvosCategoryI == true)
                applies = true;
            if (category == NvosCategory.II && rule.TriggerNvosCategoryII == true)
                applies = true;
            if (category == NvosCategory.III && rule.TriggerNvosCategoryIII == true)
                applies = true;
            if (category == NvosCategory.IV && rule.TriggerNvosCategoryIV == true)
                applies = true;

            // 2.2. Проверка по Водопользованию
            if (rule.TriggerWaterUseType.HasValue && rule.TriggerWaterUseType == waterUse)
            {
                applies = true;
            }

            // 2.3. Проверка по Побочным продуктам
            if (
                rule.TriggerHasByproducts.HasValue
                && rule.TriggerHasByproducts.Value == hasByproducts
            )
            {
                applies = true;
            }

            // 3. Если правило подошло хотя бы по одному критерию,
            // добавляем его в список
            if (applies)
            {
                applicableRules.Add(rule);
            }
        }

        // 4. Преобразуем уникальный список правил в Требования
        var requirements = applicableRules
            .Distinct() // Убираем дубликаты, если правило подошло по 2+ критериям
            .Select(rule => new EcologicalRequirement
            {
                Title = rule.GeneratedTitle,
                Basis = rule.GeneratedBasis,
                PenaltyRisk = rule.GeneratedPenaltyRisk,
                Status = RequirementStatus.NotStarted, // По умолчанию
                Deadline = null,
                ResponsiblePerson = null,
            })
            .ToList();

        return requirements;
    }
}
