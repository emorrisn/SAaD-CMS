using CMS.ComplaintService.Application.Commands;
using CMS.ComplaintService.Domain.Entities;
using CMS.ComplaintService.Domain.Enums;
using CMS.ComplaintService.Domain.Events;
using CMS.ComplaintService.Infrastructure.Data;
using CMS.ComplaintService.Infrastructure.Messaging;
using CMS.ComplaintService.Infrastructure.Repositories;

namespace CMS.ComplaintService.Application.Handlers;

public class LogComplaintHandler(
    ComplaintDbContext db,
    IComplaintRepository repository,
    IEventBus eventBus)
{
    public async Task<Guid> HandleAsync(LogComplaintCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.TenantID)) throw new ArgumentException("TenantID is required");
        if (string.IsNullOrWhiteSpace(command.Description)) throw new ArgumentException("Description is required");
        if (!command.ConsumerID.HasValue) throw new ArgumentException("ConsumerID is required (resolved externally)");

        var complaint = new Complaint
        {
            ComplaintID = Guid.NewGuid(),
            TenantID = command.TenantID,
            ConsumerID = command.ConsumerID.Value,
            Description = command.Description,
            Subject = command.Subject,
            Category = command.Category,
            Priority = command.Priority,
            Status = ComplaintStatus.New,
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(complaint, ct);
        db.ComplaintAuditHistories.Add(new ComplaintAuditHistory
        {
            AuditID = Guid.NewGuid(),
            ComplaintID = complaint.ComplaintID,
            UserID = command.ConsumerID.Value,
            Action = "Complaint Logged",
            Timestamp = DateTime.UtcNow
        });
        await repository.SaveChangesAsync(ct);

        eventBus.Publish(new ComplaintLogged(complaint));
        return complaint.ComplaintID;
    }
}
