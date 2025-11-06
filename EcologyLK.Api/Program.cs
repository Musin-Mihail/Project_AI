using System.Text; // Добавлено
using EcologyLK.Api.Data;
using EcologyLK.Api.Models;
using EcologyLK.Api.Services;
using EcologyLK.Api.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Добавлено
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // Добавлено

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration; // Добавлено

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

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("EcologyDb"));

builder
    .Services.AddIdentityCore<AppUser>(options =>
    { // Добавлены опции
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// --- ДОБАВЛЕН БЛОК JWT АУТЕНТИФИКАЦИИ ---
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = config["Jwt:SecretKey"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT SecretKey не настроен в appsettings.json");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });
builder.Services.AddAuthorization();

// --- КОНЕЦ БЛОКА ---

builder.Services.AddScoped<IRequirementGenerationService, RequirementGenerationService>();
builder.Services.AddSingleton<IArtifactStorageService, FileArtifactStorageService>();
builder.Services.AddScoped<IAuthTokenService, AuthTokenService>(); // Добавлено
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

// --- ИЗМЕНЕН БЛОК SEEDING ---
// Выполняем старый сидер (для Клиента)
DataSeeder.SeedDatabase(app);

// Выполняем новый асинхронный сидер (для Ролей и Админа)
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    // Используем .Wait() для синхронного выполнения при старте
    DataSeeder.SeedRolesAndAdminAsync(serviceProvider).Wait();
}

// --- КОНЕЦ БЛОКА ---

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);

// --- ДОБАВЛЕНЫ UseAuthentication и UseAuthorization ---
// Должны быть ДО UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

// --- КОНЕЦ БЛОКА ---

app.MapControllers();

app.Run();
