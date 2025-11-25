namespace CMS.UserService.Domain;

public enum UserRole { Agent, Support, Manager, Consumer, Admin }

public class User
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string Username { get; set; } = default!;
    public string NormalizedUsername { get; set; } = default!;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string PasswordHash { get; set; } = default!; // PBKDF2 SHA256
}