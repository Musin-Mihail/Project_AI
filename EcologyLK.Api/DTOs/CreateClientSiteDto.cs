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
    [Required]
    public int ClientId { get; set; } // К какому клиенту привязать

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required]
    public NvosCategory NvosCategory { get; set; }

    [Required]
    public WaterUseType WaterUseType { get; set; }

    [Required]
    public bool HasByproducts { get; set; }
}
