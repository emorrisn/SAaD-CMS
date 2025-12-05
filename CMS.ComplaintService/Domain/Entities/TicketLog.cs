namespace CMS.ComplaintService.Domain.Entities;

public class TicketLog
{
    public Guid TicketLogId { get; set; }
    public Guid TicketId { get; set; }
    public Guid? PerformedById { get; set; }
    public string Action { get; set; } = default!;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ticket? Ticket { get; set; }
}
