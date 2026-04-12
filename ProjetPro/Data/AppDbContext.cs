// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using ProjetPro.Models;

namespace ProjetPro.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Produit> Produits { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Convention .NET 9 — configuration des tables
        modelBuilder.Entity<Produit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom)
                  .IsRequired()
                  .HasMaxLength(100);
            entity.Property(e => e.Prix)
                  .HasColumnType("decimal(10,2)"); //format MySQL pour les prix
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Expediteur).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Destinataire).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Contenu).IsRequired().HasMaxLength(1000);
             entity.Property(e => e.Statut).HasConversion<string>();
        entity.Property(e => e.Type).HasConversion<string>();

            //statut stoké comme string en base de données(plus lisible)
             entity.HasOne(m => m.Client)
              .WithMany(c => c.Messages)
              .HasForeignKey(m => m.ClientId) // ← ClientId est la clé étrangère
              .OnDelete(DeleteBehavior.Cascade);

        });
        // Configuration Client
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);

            // ApiKey doit être unique
            entity.HasIndex(e => e.ApiKey).IsUnique();

            // Relation Client → Messages
            // entity.HasMany(c => c.Messages)
            //       .WithOne()
            //       .OnDelete(DeleteBehavior.Cascade);
        });
    }
}