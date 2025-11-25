using CMS.ComplaintService.Domain.Entities;

namespace CMS.ComplaintService.Application.Queries;

public interface IComplaintQueryService
{
    Task<Complaint?> GetByIdAsync(string tenantId, Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Complaint>> GetAllAsync(string tenantId, int skip = 0, int take = 50, CancellationToken ct = default);
}
