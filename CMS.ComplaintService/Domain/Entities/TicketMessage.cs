namespace CMS.ComplaintService.Domain.Entities;

public class TicketMessage
{
    public Guid TicketMessageId { get; set; }
    public Guid TicketId { get; set; }
    public Guid? SenderId { get; set; }
    public bool IsFromCustomer { get; set; }
    public string Body { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ticket? Ticket { get; set; }
}
