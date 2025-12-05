using CMS.ComplaintService.Domain.Entities;

namespace CMS.ComplaintService.Application.Queries;

public interface ITicketQueryService
{
    Task<Ticket?> GetByIdAsync(string tenantId, Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Ticket>> GetAllAsync(string tenantId, int skip = 0, int take = 50, CancellationToken ct = default);
}
