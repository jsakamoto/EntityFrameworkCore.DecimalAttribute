using Microsoft.EntityFrameworkCore;

namespace Toolbelt.ComponentModel.DataAnnotations.Test.Models.Inheritance;

/// <summary>
/// The base of the contexts that map the same hierarchy in different ways. Each mapping has its own context type, because EF Core caches a model per context type.
/// </summary>
public abstract class InheritanceDbContext : DbContext
{
    public DbSet<Asset> Assets { get; set; } = null!;

    public DbSet<Vehicle> Vehicles { get; set; } = null!;

    public DbSet<Furniture> Furnitures { get; set; } = null!;

    protected InheritanceDbContext(DbContextOptions options) : base(options)
    {
    }

    protected abstract void ConfigureMapping(ModelBuilder modelBuilder);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        this.ConfigureMapping(modelBuilder);
        modelBuilder.BuildDecimalColumnTypeFromAnnotations();
    }
}

/// <summary>
/// Table per hierarchy (the default mapping).
/// </summary>
public class TphDbContext(DbContextOptions options) : InheritanceDbContext(options)
{
    protected override void ConfigureMapping(ModelBuilder modelBuilder)
    {
    }
}

/// <summary>
/// Table per type.
/// </summary>
public class TptDbContext(DbContextOptions options) : InheritanceDbContext(options)
{
    protected override void ConfigureMapping(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>().UseTptMappingStrategy();
    }
}

/// <summary>
/// Table per concrete type.
/// </summary>
public class TpcDbContext(DbContextOptions options) : InheritanceDbContext(options)
{
    protected override void ConfigureMapping(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>().UseTpcMappingStrategy();
    }
}
