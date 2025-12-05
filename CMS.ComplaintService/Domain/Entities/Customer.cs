namespace CMS.ComplaintService.Domain.Entities;

public class Customer
{
    public Guid CustomerId { get; set; }
    public string TenantId { get; set; } = default!;

    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
