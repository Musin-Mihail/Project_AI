using Microsoft.AspNetCore.Identity;

namespace EcologyLK.Api.Models;

// Пользователь системы (Админ, Менеджер, Клиент)
public class AppUser : IdentityUser
{
    // Можно добавить доп. поля, например ФИО
    public string FullName { get; set; } = string.Empty;

    // Связь с клиентом (если роль "Клиент" или "Менеджер")
    public int? ClientId { get; set; }
    public Client? Client { get; set; }
}
