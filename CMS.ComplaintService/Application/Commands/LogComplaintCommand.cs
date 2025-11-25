using CMS.ComplaintService.Domain.Enums;

namespace CMS.ComplaintService.Application.Commands;

public class LogComplaintCommand
{
    public string TenantID { get; set; } = default!;

    public Guid? ConsumerID { get; set; }
    public string? ConsumerUsername { get; set; }

    public string Description { get; set; } = default!;
    public string? Subject { get; set; }
    public string? Category { get; set; }
    public PriorityLevel? Priority { get; set; }
}
