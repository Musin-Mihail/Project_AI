using EcologyLK.Api.Models;

namespace EcologyLK.Api.Data;

/// <summary>
/// Статический класс для инициализации базы данных (Seeding)
/// тестовыми данными при старте приложения.
/// </summary>
public static class DataSeeder
{
    public static void SeedDatabase(IApplicationBuilder app)
    {
        // Использование using для корректного освобождения ресурсов
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
            if (context == null)
            {
                Console.WriteLine("Не удалось получить AppDbContext для Seeding.");
                return;
            }

            // Гарантируем, что БД (in-memory) создана
            context.Database.EnsureCreated();

            // --- Инициализация Клиентов ---
            // Проверяем, есть ли уже клиенты
            if (!context.Clients.Any())
            {
                // Создаем первого клиента.
                // В InMemoryDatabase он автоматически получит Id = 1
                context.Clients.Add(
                    new Client
                    {
                        Name = "ООО \"Тестовый Клиент (ЮрЛицо)\"",
                        Inn = "1234567890",
                        Ogrn = "1234567890123",
                    }
                );

                // Сохраняем изменения, чтобы этот клиент
                // был доступен для следующих операций
                context.SaveChanges();

                Console.WriteLine("DataSeeder: Тестовый клиент (Id=1) успешно создан.");
            }
        }
    }
}
