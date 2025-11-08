using EcologyLK.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Добавлено

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

    /// <summary>
    /// (НОВОЕ) Инициализирует таблицу "Правил генерации"
    /// на основе hardcode-логики из RequirementGenerationService
    /// </summary>
    public static async Task SeedRequirementRulesAsync(IServiceProvider serviceProvider)
    {
        try
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            if (await context.RequirementRules.AnyAsync())
            {
                // Правила уже существуют, выход
                return;
            }

            // Если таблица пуста, заполняем ее
            var rules = GetHardcodedRules();
            await context.RequirementRules.AddRangeAsync(rules);
            await context.SaveChangesAsync();
            Console.WriteLine(
                $"DataSeeder: Таблица RequirementRules успешно заполнена ({rules.Count} правил)."
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"DataSeeder: Исключение при инициализации RequirementRules. {ex.Message}"
            );
        }
    }

    /// <summary>
    /// (НОВОЕ) Вспомогательный метод, дублирующий логику
    /// из RequirementGenerationService.cs
    /// </summary>
    private static List<RequirementRule> GetHardcodedRules()
    {
        var rules = new List<RequirementRule>();

        // --- 1. Требования, зависящие от категории (Таблица из "Критерии...") ---

        // Эти требования есть у I, II, III (кроме IV)
        rules.Add(
            new RequirementRule
            {
                Description = "Декларация НВОС (Кат I, II, III)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                GeneratedTitle = "Декларация о плате за негативное воздействие на окружающую среду",
                GeneratedBasis = "КоАП РФ 8.41/8.41.1",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.41): 50-100 тыс. руб.",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "Отчет ПЭК (Кат I, II, III)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                GeneratedTitle =
                    "Отчет по программе производственного экологического контроля (ПЭК)",
                GeneratedBasis = "ФЗ-7, приказы РПН",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.5): 20-80 тыс. руб.",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "Журналы учета выбросов (Кат I, II, III)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                GeneratedTitle =
                    "Журналы учета стационарных источников выбросов и их характеристик",
                GeneratedBasis = "Закон об атм. воздухе",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.1): 20-100 тыс. руб.",
            }
        );

        // Эти требования есть у всех (I, II, III, IV)
        rules.Add(
            new RequirementRule
            {
                Description = "Журналы учета отходов (Все Кат)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                TriggerNvosCategoryIV = true,
                GeneratedTitle = "Журналы учета движения отходов производства и потребления",
                GeneratedBasis = "Приказ Минприроды",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.2): 100-250 тыс. руб.",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "Инвентаризация выбросов (Все Кат)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                TriggerNvosCategoryIV = true,
                GeneratedTitle =
                    "Отчет инвентаризации выбросов вредных (загрязняющих) веществ в атмосферу",
                GeneratedBasis = "ФЗ-7",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.21 ч.1): 100-200 тыс. руб.",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "Инвентаризация отходов (Все Кат)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                TriggerNvosCategoryIV = true,
                GeneratedTitle = "Отчет инвентаризации отходов производства и потребления",
                GeneratedBasis = "ФЗ-89",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.2): 100-250 тыс. руб.",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "Паспорта на отходы I-IV (Все Кат)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                TriggerNvosCategoryIV = true,
                GeneratedTitle = "Паспорта на отходы I-IV класса опасности",
                GeneratedBasis = "ФЗ-89",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.2): 100-250 тыс. руб.",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "Инструкции по отходам (Все Кат)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                TriggerNvosCategoryIV = true,
                GeneratedTitle = "Инструкции по обращению с отходами и приказы",
                GeneratedBasis = "ФЗ-89",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.2): 100-250 тыс. руб.",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "Проект СЗЗ (Все Кат)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                TriggerNvosCategoryIV = true,
                GeneratedTitle =
                    "Проект санитарно-защитной зоны (СЗЗ) и Решение об установлении СЗЗ",
                GeneratedBasis = "СанПиН",
                GeneratedPenaltyRisk = "Штраф (КоАП 6.3): 10-20 тыс. руб. / приостановка",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "2-ТП (воздух) (Все Кат)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                TriggerNvosCategoryIV = true,
                GeneratedTitle = "Статотчетность по форме 2-ТП (воздух) (при выбросе > 5 т/год)",
                GeneratedBasis = "Приказ Росстата",
                GeneratedPenaltyRisk = "Штраф (КоАП 13.19): 20-70 тыс. руб.",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "2-ТП (отходы) (Все Кат)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                TriggerNvosCategoryIV = true,
                GeneratedTitle =
                    "Статотчетность по форме 2-ТП (отходы) (при образовании > 100 т/год)",
                GeneratedBasis = "Приказ Росстата",
                GeneratedPenaltyRisk = "Штраф (КоАП 13.19): 20-70 тыс. руб.",
            }
        );

        // Требования для I, II, III
        rules.Add(
            new RequirementRule
            {
                Description = "СЭЗ на проект НДВ (Кат I, II, III)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                GeneratedTitle = "Экспертное заключение и СЭЗ на проект НДВ",
                GeneratedBasis = "Роспотребнадзор",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "План НМУ (Кат I, II, III)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                GeneratedTitle = "План мероприятий неблагоприятных метеорологических условий (НМУ)",
                GeneratedBasis = "ФЗ-7",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.1): 20-100 тыс. руб.",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "Программа ППЭК (Кат I, II, III)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                GeneratedTitle = "Программа производственного экологического контроля (ППЭК)",
                GeneratedBasis = "ФЗ-7",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.5): 20-80 тыс. руб.",
            }
        );

        // Требования для I, II
        rules.Add(
            new RequirementRule
            {
                Description = "НООЛР (Кат I, II)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                GeneratedTitle = "Нормативы образования отходов и лимиты на их размещение (НООЛР)",
                GeneratedBasis = "ФЗ-89",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.2): 100-250 тыс. руб.",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "НДВ (Кат I, II)",
                TriggerNvosCategoryI = true,
                TriggerNvosCategoryII = true,
                GeneratedTitle = "Нормативы допустимых выбросов (НДВ)",
                GeneratedBasis = "КоАП РФ 8.21 ч.1",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.21 ч.1): 100-200 тыс. руб.",
            }
        );

        // Специфичные по категориям
        rules.Add(
            new RequirementRule
            {
                Description = "КЭР (Кат I)",
                TriggerNvosCategoryI = true,
                GeneratedTitle = "Комплексное экологическое разрешение (КЭР)",
                GeneratedBasis = "ФЗ-7",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.47): 50-100 тыс. руб.",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "ДВОС (Кат II)",
                TriggerNvosCategoryII = true,
                GeneratedTitle = "Декларация о воздействии на окружающую среду (ДВОС)",
                GeneratedBasis = "ФЗ-7",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.5): 20-80 тыс. руб.",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "НДВ для I-II кл. (Кат III)",
                TriggerNvosCategoryIII = true,
                GeneratedTitle = "Нормативы выбросов для веществ І, ІІ класса опасности",
                GeneratedBasis = "ФЗ-7",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.21 ч.1): 100-200 тыс. руб.",
            }
        );

        // --- 2. Экологические требования (Водопользование) ---
        rules.Add(
            new RequirementRule
            {
                Description = "Вода (Скважина)",
                TriggerWaterUseType = WaterUseType.Well,
                GeneratedTitle = "Лицензия на право пользования недрами",
                GeneratedBasis = "Закон о недрах",
                GeneratedPenaltyRisk = "Штраф (КоАП 7.3 ч.1): 800-1000 тыс. руб.",
            }
        );
        rules.Add(
            new RequirementRule
            {
                Description = "Вода (Река)",
                TriggerWaterUseType = WaterUseType.River,
                GeneratedTitle =
                    "Решение на право пользования водным объектом и/или Договор водопользования",
                GeneratedBasis = "Водный кодекс РФ",
                GeneratedPenaltyRisk = "Штраф (КоАП 7.6): 50-100 тыс. руб.",
            }
        );

        // --- 3. Побочный продукт ---
        rules.Add(
            new RequirementRule
            {
                Description = "Побочный продукт (Навоз)",
                TriggerHasByproducts = true,
                GeneratedTitle =
                    "Технические условия 'Удобрения органические на основе побочной продукции животноводства'",
                GeneratedBasis = "ГОСТ/ТУ",
                GeneratedPenaltyRisk = "Риск (ФЗ-248): 250-350 тыс. руб.",
            }
        );

        // --- 4. Общее для всех (не из таблицы) ---
        rules.Add(
            new RequirementRule
            {
                Description = "Обучение (Все)",
                TriggerNvosCategoryI = true, // Хак: чтобы срабатывало для всех
                TriggerNvosCategoryII = true,
                TriggerNvosCategoryIII = true,
                TriggerNvosCategoryIV = true,
                GeneratedTitle = "Обучение ответственных лиц (Удостоверение)",
                GeneratedBasis = "ФЗ-7",
                GeneratedPenaltyRisk = "Штраф (КоАП 8.1): 20-100 тыс. руб.",
            }
        );

        return rules;
    }
}
