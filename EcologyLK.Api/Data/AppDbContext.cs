using EcologyLK.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcologyLK.Api.Data;

/// <summary>
/// Контекст базы данных Entity Framework Core.
/// Использует IdentityDbContext для интеграции с ASP.NET Identity.
/// </summary>
public class AppDbContext : IdentityDbContext<AppUser>
{
    /// <summary>
    /// Конструктор AppDbContext.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    /// <summary>
    /// Клиенты (ЮрЛица).
    /// </summary>
    public DbSet<Client> Clients { get; set; }

    /// <summary>
    /// Площадки клиентов.
    /// </summary>
    public DbSet<ClientSite> ClientSites { get; set; }

    /// <summary>
    /// Экологические требования ("Карта требований").
    /// </summary>
    public DbSet<EcologicalRequirement> EcologicalRequirements { get; set; }

    /// <summary>
    /// Артефакты (файлы).
    /// </summary>
    public DbSet<Artifact> Artifacts { get; set; }

    /// <summary>
    /// Финансовые документы (Договора, Счета, Акты).
    /// </summary>
    public DbSet<FinancialDocument> FinancialDocuments { get; set; }

    /// <summary>
    /// Справочник НПА (Нормативно-правовые акты).
    /// </summary>
    public DbSet<LegalAct> LegalActs { get; set; }

    /// <summary>
    /// Справочник "Правил генерации требований".
    /// </summary>
    public DbSet<RequirementRule> RequirementRules { get; set; }

    /// <summary>
    /// Настройка моделей и их связей (Fluent API).
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Обязательно для Identity

        // Client -> ClientSite (Один ко многим)
        builder
            .Entity<Client>()
            .HasMany(c => c.Sites)
            .WithOne(s => s.Client)
            .HasForeignKey(s => s.ClientId)
            .OnDelete(DeleteBehavior.Cascade); // При удалении клиента удалить и его площадки

        // ClientSite -> EcologicalRequirement (Один ко многим)
        builder
            .Entity<ClientSite>()
            .HasMany(s => s.Requirements)
            .WithOne(r => r.ClientSite)
            .HasForeignKey(r => r.ClientSiteId)
            .OnDelete(DeleteBehavior.Cascade);

        // ClientSite -> Artifact (Один ко многим)
        builder
            .Entity<ClientSite>()
            .HasMany(s => s.Artifacts)
            .WithOne(a => a.ClientSite)
            .HasForeignKey(a => a.ClientSiteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь Артефакта с Требованием (опционально)
        builder
            .Entity<Artifact>()
            .HasOne(a => a.EcologicalRequirement)
            .WithMany() // У требования может быть много артефактов
            .HasForeignKey(a => a.EcologicalRequirementId)
            .OnDelete(DeleteBehavior.SetNull); // При удалении требования, артефакт остается, но связь сбрасывается

        // ClientSite -> FinancialDocument (Один ко многим)
        builder
            .Entity<ClientSite>()
            .HasMany(s => s.FinancialDocuments)
            .WithOne(d => d.ClientSite)
            .HasForeignKey(d => d.ClientSiteId)
            .OnDelete(DeleteBehavior.Cascade);

        // AppUser -> Client (Один ко многим, опционально)
        builder
            .Entity<Client>()
            .HasMany(c => c.Users)
            .WithOne(u => u.Client)
            .HasForeignKey(u => u.ClientId)
            .OnDelete(DeleteBehavior.SetNull); // Если клиент удален, у пользователя просто сбросится ClientId
    }
}
