using CMS.ComplaintService.Domain.Entities;
using CMS.ComplaintService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMS.ComplaintService.Application.Queries;

public class TicketQueryService(TicketDbContext db) : ITicketQueryService
{
    public Task<Ticket?> GetByIdAsync(string tenantId, Guid id, CancellationToken ct = default)
    {
        return db.Tickets
            .AsNoTracking()
            .Include(t => t.Customer)
            .Include(t => t.Messages)
            .Include(t => t.Logs)
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.TicketId == id, ct);
    }

    public async Task<IReadOnlyList<Ticket>> GetAllAsync(string tenantId, int skip = 0, int take = 50, CancellationToken ct = default)
    {
        return await db.Tickets
            .AsNoTracking()
            .Where(t => t.TenantId == tenantId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }
}
