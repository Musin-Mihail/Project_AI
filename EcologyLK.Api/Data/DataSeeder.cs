using EcologyLK.Api.Models;
using Microsoft.AspNetCore.Identity; // Добавлено

namespace EcologyLK.Api.Data;

/// <summary>
/// Статический класс для инициализации базы данных (Seeding)
/// тестовыми данными при старте приложения.
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Старый метод для сидинга Клиента
    /// </summary>
    public static void SeedDatabase(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
            if (context == null)
            {
                Console.WriteLine("Не удалось получить AppDbContext для Seeding.");
                return;
            }

            context.Database.EnsureCreated();

            if (!context.Clients.Any())
            {
                context.Clients.Add(
                    new Client
                    {
                        Name = "ООО \"Тестовый Клиент (ЮрЛицо)\"",
                        Inn = "1234567890",
                        Ogrn = "1234567890123",
                    }
                );
                context.SaveChanges();
                Console.WriteLine("DataSeeder: Тестовый клиент (Id=1) успешно создан.");
            }
        }
    }

    /// <summary>
    /// Новый асинхронный метод для инициализации Ролей и Админа
    /// </summary>
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        try
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            string[] roleNames = { "Admin", "Manager", "Client" };
            IdentityResult roleResult;

            // 1. Создаем роли
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    Console.WriteLine($"DataSeeder: Роль '{roleName}' создана.");
                }
            }

            // 2. Создаем Admin-пользователя
            var adminEmail = "admin@ecology.lk";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Администратор Системы",
                    EmailConfirmed = true, // Подтверждаем Email
                };

                // TODO: Пароль из appsettings
                var result = await userManager.CreateAsync(newAdminUser, "AdminP@ssw0rd1!");
                if (result.Succeeded)
                {
                    // 3. Присваиваем роль "Admin"
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                    Console.WriteLine("DataSeeder: Пользователь 'Admin' успешно создан.");
                }
                else
                {
                    Console.WriteLine(
                        $"DataSeeder: Ошибка при создании Admin: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"DataSeeder: Исключение при инициализации ролей/админа. {ex.Message}"
            );
        }
    }
}
