using CMS.TenantService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CMS.TenantService.Infrastructure.Data;

public class TenantDbContext(DbContextOptions<TenantDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Tenant>(e =>
        {
            e.ToTable("Tenants");
            e.HasKey(x => x.TenantId);
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.Code).IsRequired();
            e.Property(x => x.Name).IsRequired();
            e.HasIndex(x => x.IsActive);
        });
    }
}