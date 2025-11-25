namespace CMS.ComplaintService.Domain.Entities;

public class ComplaintAuditHistory
{
    public Guid AuditID { get; set; }
    public Guid ComplaintID { get; set; }
    public Guid UserID { get; set; }
    public string Action { get; set; } = default!;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
