using CMS.ComplaintService.Application.Commands;
using CMS.ComplaintService.Domain.Entities;
using CMS.ComplaintService.Domain.Events;
using CMS.ComplaintService.Infrastructure.Messaging;
using CMS.ComplaintService.Infrastructure.Repositories;

namespace CMS.ComplaintService.Application.Handlers;

public class CreateTicketHandler(ITicketRepository repository, IEventBus eventBus)
{
    public async Task<Guid> HandleAsync(CreateTicketCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.TenantId)) throw new ArgumentException("TenantId is required");
        if (command.CustomerId == Guid.Empty) throw new ArgumentException("CustomerId is required (resolved externally)");
        if (string.IsNullOrWhiteSpace(command.Subject)) throw new ArgumentException("Subject is required");
        if (string.IsNullOrWhiteSpace(command.Description)) throw new ArgumentException("Description is required");

        var ticket = new Ticket
        {
            TicketId = Guid.NewGuid(),
            TenantId = command.TenantId,
            CustomerId = command.CustomerId,
            AssignedId = command.AssignedId,
            Subject = command.Subject,
            Description = command.Description,
            Category = command.Category,
            Priority = command.Priority,
            Status = command.Status,
            Source = command.Source,
            IsEscalated = command.IsEscalated,
            ResolutionNotes = command.ResolutionNotes,
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddTicketAsync(ticket, ct);
        await repository.AddTicketLogAsync(new TicketLog
        {
            TicketLogId = Guid.NewGuid(),
            TicketId = ticket.TicketId,
            PerformedById = command.AssignedId ?? command.CustomerId,
            Action = "Ticket Created",
            Notes = command.ResolutionNotes,
            CreatedAt = DateTime.UtcNow
        }, ct);

        await repository.SaveChangesAsync(ct);
        eventBus.Publish(new TicketLogged(ticket));

        return ticket.TicketId;
    }
}
