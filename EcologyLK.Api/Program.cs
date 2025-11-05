using EcologyLK.Api.Data;
using EcologyLK.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- НАЧАЛО: Добавлено ИИ ---

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

// --- КОНЕЦ: Добавлено ИИ ---

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

// --- НАЧАЛО: Добавлено ИИ ---

// 4. Включение CORS
app.UseCors(MyAllowSpecificOrigins);

// --- КОНЕЦ: Добавлено ИИ ---

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

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
