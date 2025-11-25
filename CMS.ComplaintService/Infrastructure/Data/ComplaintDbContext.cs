using CMS.ComplaintService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.ComplaintService.Infrastructure.Data;

public class ComplaintDbContext(DbContextOptions<ComplaintDbContext> options) : DbContext(options)
{
    public DbSet<Complaint> Complaints => Set<Complaint>();
    public DbSet<ComplaintAuditHistory> ComplaintAuditHistories => Set<ComplaintAuditHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Complaint>(b =>
        {
            b.HasKey(c => c.ComplaintID);
            b.Property(c => c.TenantID).IsRequired();
            b.Property(c => c.Description).IsRequired();
            b.HasIndex(c => new { c.TenantID, c.Status });
        });

        modelBuilder.Entity<ComplaintAuditHistory>(b =>
        {
            b.HasKey(a => a.AuditID);
            b.Property(a => a.Action).IsRequired();
            b.HasIndex(a => a.ComplaintID);
        });
    }
}
