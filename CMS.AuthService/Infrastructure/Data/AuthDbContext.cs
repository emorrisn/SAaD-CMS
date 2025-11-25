using CMS.AuthService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CMS.AuthService.Infrastructure.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<Session> Sessions => Set<Session>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Session>(e =>
        {
            e.ToTable("Sessions");
            e.HasKey(x => x.SessionId);
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.TokenHash).IsUnique();
            e.Property(x => x.TokenHash).IsRequired();
            e.Property(x => x.ExpiresAtUtc).IsRequired();
        });
    }
}