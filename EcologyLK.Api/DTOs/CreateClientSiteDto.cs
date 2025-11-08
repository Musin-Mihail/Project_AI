using System.ComponentModel.DataAnnotations;
using EcologyLK.Api.Models;

namespace EcologyLK.Api.DTOs;

/// <summary>
/// DTO для "Анкеты".
/// Данные, которые мы ожидаем от клиента (Angular)
/// при создании новой площадки.
/// </summary>
public class CreateClientSiteDto
{
    /// <summary>
    /// К какому клиенту (юрлицу) привязать площадку.
    /// </summary>
    [Required]
    public int ClientId { get; set; }

    /// <summary>
    /// Название площадки.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Адрес площадки.
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Категория НВОС.
    /// </summary>
    [Required]
    public NvosCategory NvosCategory { get; set; }

    /// <summary>
    /// Тип водопользования.
    /// </summary>
    [Required]
    public WaterUseType WaterUseType { get; set; }

    /// <summary>
    /// Признак наличия побочных продуктов (навоз/помет).
    /// </summary>
    [Required]
    public bool HasByproducts { get; set; }
}
