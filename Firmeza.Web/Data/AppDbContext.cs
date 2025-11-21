using Microsoft.EntityFrameworkCore;
using Firmeza.Web.Models.Entities;
namespace Firmeza.Web.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<SaleDetail> SaleDetails { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure decimal precision to avoid overflow
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Sale>()
            .Property(s => s.Total)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Sale>()
            .Property(s => s.Vat)
            .HasPrecision(18, 2);

        modelBuilder.Entity<SaleDetail>()
            .Property(sd => sd.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Product>().ToTable("products");
        modelBuilder.Entity<Sale>().ToTable("sales");
        modelBuilder.Entity<SaleDetail>().ToTable("sale_details");



        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<IdentityRole>().ToTable("roles");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("user_roles");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("user_logins");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("user_tokens");
    }

    public DbSet<Firmeza.Web.Models.Entities.Product> Product { get; set; } = default!;
}