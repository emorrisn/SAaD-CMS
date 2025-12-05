using CMS.ComplaintService.Domain.Enums;

namespace CMS.ComplaintService.Domain.Entities;

public class Ticket
{
    public Guid TicketId { get; set; }
    public string TenantId { get; set; } = default!;

    public string Subject { get; set; } = default!;
    public string Description { get; set; } = default!;

    public Guid CustomerId { get; set; }
    public Guid? AssignedId { get; set; }

    public string? ResolutionNotes { get; set; }
    public bool IsEscalated { get; set; }
    public string? Category { get; set; }
    public TicketPriority? Priority { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.New;
    public string? Source { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public Customer? Customer { get; set; }
    public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    public ICollection<TicketLog> Logs { get; set; } = new List<TicketLog>();
}
