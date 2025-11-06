using EcologyLK.Api.Models;

namespace EcologyLK.Api.Services;

/// <summary>
/// Интерфейс сервиса, отвечающего за бизнес-логику
/// генерации "Карты требований" на основе параметров площадки.
/// </summary>
public interface IRequirementGenerationService
{
    /// <summary>
    /// Генерирует список экологических требований
    /// </summary>
    /// <param name="category">Категория НВОС</param>
    /// <param name="waterUse">Тип водопользования</param>
    /// <param name="hasByproducts">Наличие побочных продуктов</param>
    /// <returns>Список сгенерированных требований</returns>
    List<EcologicalRequirement> GenerateRequirements(
        NvosCategory category,
        WaterUseType waterUse,
        bool hasByproducts
    );
}
