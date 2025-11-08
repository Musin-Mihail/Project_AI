using System.ComponentModel.DataAnnotations;

namespace EcologyLK.Api.Models;

/// <summary>
/// Клиент (Юрлицо / Бренд).
/// </summary>
public class Client
{
    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Наименование организации.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ИНН (10 или 12 знаков).
    /// </summary>
    [Required]
    [StringLength(12)]
    public string Inn { get; set; } = string.Empty;

    /// <summary>
    /// ОГРН (13 или 15 знаков).
    /// </summary>
    [StringLength(15)]
    public string Ogrn { get; set; } = string.Empty;

    /// <summary>
    /// Навигационное свойство: Список площадок этого клиента.
    /// </summary>
    public List<ClientSite> Sites { get; set; } = new();

    /// <summary>
    /// Навигационное свойство: Пользователи, привязанные к этому клиенту.
    /// </summary>
    public List<AppUser> Users { get; set; } = new();
}
