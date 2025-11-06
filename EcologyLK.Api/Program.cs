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

// 5. Регистрация AutoMapper (Добавлено ИИ)
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 6. Регистрация Контроллеров (Добавлено ИИ)
builder.Services.AddControllers();

// --- КОНЕЦ: Изменения ИИ ---

// Add services to the container.
// Learn more about configuring OpenAPI at [https://aka.ms/aspnet/openapi](https://aka.ms/aspnet/openapi)
builder.Services.AddOpenApi();

var app = builder.Build();

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

// ----- НАЧАЛО: Удалено ИИ (старый пример WeatherForecast) -----
/*
var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};

app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    )
    .WithName("GetWeatherForecast");
*/
// ----- КОНЕЦ: Удалено ИИ -----

app.Run();

// ----- НАЧАЛО: Удалено ИИ (старый record) -----
/*
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
*/
// ----- КОНЕЦ: Удалено ИИ -----
