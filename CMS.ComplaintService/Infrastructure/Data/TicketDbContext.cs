using CMS.ComplaintService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.ComplaintService.Infrastructure.Data;

public class TicketDbContext(DbContextOptions<TicketDbContext> options) : DbContext(options)
{
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<TicketLog> TicketLogs => Set<TicketLog>();
    public DbSet<TicketMessage> TicketMessages => Set<TicketMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(b =>
        {
            b.HasKey(t => t.TicketId);
            b.Property(t => t.TenantId).IsRequired();
            b.Property(t => t.Subject).IsRequired();
            b.Property(t => t.Description).IsRequired();
            b.Property(t => t.Status).IsRequired();
            b.HasIndex(t => new { t.TenantId, t.Status });

            b.HasOne(t => t.Customer)
                .WithMany()
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Customer>(b =>
        {
            b.HasKey(c => c.CustomerId);
            b.Property(c => c.TenantId).IsRequired();
            b.Property(c => c.Email).IsRequired();
            b.Property(c => c.FirstName).IsRequired();
            b.Property(c => c.LastName).IsRequired();
            b.HasIndex(c => new { c.TenantId, c.Email }).IsUnique();
        });

        modelBuilder.Entity<TicketLog>(b =>
        {
            b.HasKey(l => l.TicketLogId);
            b.Property(l => l.Action).IsRequired();
            b.HasIndex(l => l.TicketId);
            b.HasOne(l => l.Ticket)
                .WithMany(t => t.Logs)
                .HasForeignKey(l => l.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TicketMessage>(b =>
        {
            b.HasKey(m => m.TicketMessageId);
            b.Property(m => m.Body).IsRequired();
            b.HasIndex(m => m.TicketId);
            b.HasOne(m => m.Ticket)
                .WithMany(t => t.Messages)
                .HasForeignKey(m => m.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
