using CMS.UserService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CMS.UserService.Infrastructure.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.UserId);
            e.Property(x => x.Username).IsRequired();
            e.Property(x => x.NormalizedUsername)
                .HasComputedColumnSql("upper(\"Username\")", stored: true);
            e.Property(x => x.PasswordHash).IsRequired();
            e.HasIndex(x => new { x.TenantId, x.NormalizedUsername }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.Role });
        });
    }
}