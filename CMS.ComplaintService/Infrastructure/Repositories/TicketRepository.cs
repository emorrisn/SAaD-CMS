using CMS.ComplaintService.Domain.Entities;
using CMS.ComplaintService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMS.ComplaintService.Infrastructure.Repositories;

public class TicketRepository(TicketDbContext db) : ITicketRepository
{
    public async Task AddTicketAsync(Ticket ticket, CancellationToken ct = default)
    {
        await db.Tickets.AddAsync(ticket, ct);
    }

    public async Task AddTicketLogAsync(TicketLog log, CancellationToken ct = default)
    {
        await db.TicketLogs.AddAsync(log, ct);
    }

    public async Task AddTicketMessageAsync(TicketMessage message, CancellationToken ct = default)
    {
        await db.TicketMessages.AddAsync(message, ct);
    }

    public Task<Ticket?> GetTicketByIdAsync(string tenantId, Guid id, CancellationToken ct = default)
    {
        return db.Tickets
            .AsNoTracking()
            .Include(t => t.Customer)
            .Include(t => t.Messages)
            .Include(t => t.Logs)
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.TicketId == id, ct);
    }

    public Task<Customer?> GetCustomerByIdAsync(string tenantId, Guid id, CancellationToken ct = default)
    {
        return db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.TenantId == tenantId && c.CustomerId == id, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
