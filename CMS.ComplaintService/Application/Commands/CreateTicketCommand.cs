using CMS.ComplaintService.Domain.Enums;

namespace CMS.ComplaintService.Application.Commands;

public class CreateTicketCommand
{
    public string TenantId { get; set; } = default!;

    public Guid CustomerId { get; set; }
    public Guid? AssignedId { get; set; }

    public string Subject { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? Category { get; set; }
    public TicketPriority? Priority { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.New;
    public string? Source { get; set; }
    public bool IsEscalated { get; set; }
    public string? ResolutionNotes { get; set; }
}
