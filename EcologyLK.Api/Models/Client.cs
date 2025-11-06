using System.ComponentModel.DataAnnotations;

namespace EcologyLK.Api.Models;

// Клиент (Юрлицо / Бренд)
public class Client
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(12)] // ИНН 10 или 12
    public string Inn { get; set; } = string.Empty;

    [StringLength(15)] // ОГРН 13 или 15
    public string Ogrn { get; set; } = string.Empty;

    // Связь: Один клиент может иметь много площадок
    public List<ClientSite> Sites { get; set; } = new();

    // Связь: Пользователи, привязанные к этому клиенту
    public List<AppUser> Users { get; set; } = new();
}
