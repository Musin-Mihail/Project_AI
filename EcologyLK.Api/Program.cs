using EcologyLK.Api.Data;
using EcologyLK.Api.Models;
using EcologyLK.Api.Services; // Добавлено ИИ
using EcologyLK.Api.Utils; // Добавлено ИИ
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- НАЧАЛО: Изменения ИИ ---

// 1. Определение политики CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowSpecificOrigins,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200") // URL клиента Angular
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
});

// 2. Регистрация DbContext (используем In-Memory DB для MVP)
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("EcologyDb"));

// 3. Регистрация Identity (система пользователей и ролей)
builder
    .Services.AddIdentityCore<AppUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// 4. Регистрация сервиса генерации требований (Добавлено ИИ)
builder.Services.AddScoped<IRequirementGenerationService, RequirementGenerationService>();

// --- Добавлено ИИ: Регистрация сервиса хранения артефактов ---
builder.Services.AddSingleton<IArtifactStorageService, FileArtifactStorageService>();

// --- Конец ---

// 5. Регистрация AutoMapper (Добавлено ИИ)
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 6. Регистрация Контроллеров (Добавлено ИИ)
builder.Services.AddControllers();

// --- КОНЕЦ: Изменения ИИ ---

// Add services to the container.
// Learn more about configuring OpenAPI at [https://aka.ms/aspnet/openapi](https://aka.ms/aspnet/openapi)
builder.Services.AddOpenApi();

var app = builder.Build();

// --- НАЧАЛО: Добавлено ИИ (Этап 10) ---
// Инициализируем БД тестовыми данными (создаем Client c Id = 1)
// Это исправит ошибку "Client with Id 1 not found"
DataSeeder.SeedDatabase(app);

// --- КОНЕЦ: Добавлено ИИ (Этап 10) ---
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// --- НАЧАЛО: Изменения ИИ ---

// 7. Включение CORS
app.UseCors(MyAllowSpecificOrigins);

// 8. Подключение API контроллеров (Добавлено ИИ)
app.MapControllers();

// --- КОНЕЦ: Изменения ИИ ---

app.Run();
