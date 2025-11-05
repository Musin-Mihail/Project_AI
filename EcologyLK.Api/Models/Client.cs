namespace EcologyLK.Api.Models;

// Клиент (Юрлицо / Бренд)
public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Inn { get; set; } = string.Empty;
    public string Ogrn { get; set; } = string.Empty;

    // Связь: Один клиент может иметь много площадок
    public List<ClientSite> Sites { get; set; } = new();

    // Связь: Пользователи, привязанные к этому клиенту
    public List<AppUser> Users { get; set; } = new();
}
