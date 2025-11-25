using CMS.ComplaintService.Domain.Entities;
using CMS.ComplaintService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMS.ComplaintService.Application.Queries;

public class ComplaintQueryService(ComplaintDbContext db) : IComplaintQueryService
{
    public Task<Complaint?> GetByIdAsync(string tenantId, Guid id, CancellationToken ct = default)
    {
        return db.Complaints.AsNoTracking().FirstOrDefaultAsync(c => c.TenantID == tenantId && c.ComplaintID == id, ct);
    }

    public async Task<IReadOnlyList<Complaint>> GetAllAsync(string tenantId, int skip = 0, int take = 50, CancellationToken ct = default)
    {
        return await db.Complaints.AsNoTracking()
            .Where(c => c.TenantID == tenantId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip).Take(take)
            .ToListAsync(ct);
    }
}
