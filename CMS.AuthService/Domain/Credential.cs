namespace CMS.AuthService.Domain;

public class Credential
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string PasswordHash { get; set; } = default!; // PBKDF2
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastPasswordChangeUtc { get; set; }
}

public class RefreshToken
{
    public Guid RefreshTokenId { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string TokenHash { get; set; } = default!; // hash of raw token
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAtUtc { get; set; }
}