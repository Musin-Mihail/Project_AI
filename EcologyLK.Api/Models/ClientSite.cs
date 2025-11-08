namespace EcologyLK.Api.Models;

/// <summary>
/// Площадка/Объект клиента.
/// Ключевая сущность, к которой привязаны требования и артефакты.
/// </summary>
public class ClientSite
{
    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название площадки.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Адрес площадки.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Категория НВОС.
    /// </summary>
    public NvosCategory NvosCategory { get; set; }

    /// <summary>
    /// Тип водопользования.
    /// </summary>
    public WaterUseType WaterUseType { get; set; }

    /// <summary>
    /// Признак наличия побочных продуктов (навоз/помет).
    /// </summary>
    public bool HasByproducts { get; set; }

    /// <summary>
    /// ID клиента (юрлица), которому принадлежит площадка.
    /// </summary>
    public int ClientId { get; set; }

    /// <summary>
    /// Навигационное свойство для Client.
    /// </summary>
    public Client? Client { get; set; }

    /// <summary>
    /// Навигационное свойство: "Карта требований" для этой площадки.
    /// </summary>
    public List<EcologicalRequirement> Requirements { get; set; } = new();

    /// <summary>
    /// Навигационное свойство: Артефакты (файлы) для этой площадки.
    /// </summary>
    public List<Artifact> Artifacts { get; set; } = new();

    /// <summary>
    /// Навигационное свойство: Финансовые документы для этой площадки.
    /// </summary>
    public List<FinancialDocument> FinancialDocuments { get; set; } = new();
}
