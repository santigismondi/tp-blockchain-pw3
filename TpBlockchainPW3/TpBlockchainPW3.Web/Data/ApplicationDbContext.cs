using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TpBlockchainPW3.Web.Models;

namespace TpBlockchainPW3.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Cada DbSet = una tabla
    public DbSet<Documento> Documentos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);   // IMPORTANTE: configura las tablas de Identity

        // RN-03: no pueden existir dos documentos con la misma huella
        builder.Entity<Documento>()
            .HasIndex(d => d.HashSha256)
            .IsUnique();

        // RN-04: el código es único
        builder.Entity<Documento>()
            .HasIndex(d => d.Codigo)
            .IsUnique();

        // Nota con 2 decimales (ej. 10.00)
        builder.Entity<Documento>()
            .Property(d => d.Nota)
            .HasPrecision(4, 2);
    }
}