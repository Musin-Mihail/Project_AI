using EcologyLK.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcologyLK.Api.Data;

public class AppDbContext : IdentityDbContext<AppUser> // Используем IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    // Регистрация наших моделей
    public DbSet<Client> Clients { get; set; }
    public DbSet<ClientSite> ClientSites { get; set; }
    public DbSet<EcologicalRequirement> EcologicalRequirements { get; set; }
    public DbSet<Artifact> Artifacts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Обязательно для Identity

        // Настройка связей

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

        // AppUser -> Client (Один ко многим, опционально)
        builder
            .Entity<Client>()
            .HasMany(c => c.Users)
            .WithOne(u => u.Client)
            .HasForeignKey(u => u.ClientId)
            .OnDelete(DeleteBehavior.SetNull); // Если клиент удален, у пользователя просто сбросится ClientId
    }
}
