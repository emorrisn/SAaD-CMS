using CMS.ComplaintService.Domain.Entities;
using CMS.ComplaintService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMS.ComplaintService.Infrastructure.Repositories;

public class ComplaintRepository(ComplaintDbContext db) : IComplaintRepository
{
    public async Task AddAsync(Complaint complaint, CancellationToken ct = default)
    {
        await db.Complaints.AddAsync(complaint, ct);
    }

    public Task<Complaint?> GetByIdAsync(string tenantId, Guid id, CancellationToken ct = default)
    {
        return db.Complaints.AsNoTracking().FirstOrDefaultAsync(c => c.TenantID == tenantId && c.ComplaintID == id, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
