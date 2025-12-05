using CMS.ComplaintService.Domain.Entities;

namespace CMS.ComplaintService.Infrastructure.Repositories;

public interface ITicketRepository
{
    Task AddTicketAsync(Ticket ticket, CancellationToken ct = default);
    Task AddTicketLogAsync(TicketLog log, CancellationToken ct = default);
    Task AddTicketMessageAsync(TicketMessage message, CancellationToken ct = default);
    Task<Ticket?> GetTicketByIdAsync(string tenantId, Guid id, CancellationToken ct = default);
    Task<Customer?> GetCustomerByIdAsync(string tenantId, Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
