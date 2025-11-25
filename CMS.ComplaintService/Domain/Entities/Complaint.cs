using CMS.ComplaintService.Domain.Enums;

namespace CMS.ComplaintService.Domain.Entities;

public class Complaint
{
    public Guid ComplaintID { get; set; }
    public string TenantID { get; set; } = default!;

    public Guid ConsumerID { get; set; }
    public Guid? AgentID { get; set; }
    public Guid? SupportPersonID { get; set; }

    public ComplaintStatus Status { get; set; } = ComplaintStatus.New;

    public string? Subject { get; set; }
    public string Description { get; set; } = default!;
    public string? Category { get; set; }
    public PriorityLevel? Priority { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}
