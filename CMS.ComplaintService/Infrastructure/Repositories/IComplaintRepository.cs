using CMS.ComplaintService.Domain.Entities;

namespace CMS.ComplaintService.Infrastructure.Repositories;

public interface IComplaintRepository
{
    Task AddAsync(Complaint complaint, CancellationToken ct = default);
    Task<Complaint?> GetByIdAsync(string tenantId, Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
