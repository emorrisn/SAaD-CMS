using CMS.AuthService.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CMS.AuthService.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _db;
    private readonly IConfiguration _cfg;
    public AuthController(AuthDbContext db, IConfiguration cfg) { _db = db; _cfg = cfg; }

    [HttpPost("login")]
public async Task<IActionResult> Login(LoginDto dto, [FromServices] IHttpClientFactory httpClientFactory, CancellationToken ct)
{
    if (string.IsNullOrWhiteSpace(dto.TenantId) || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        return BadRequest();

    string tenantsBase = _cfg["TENANTS_BASE_URL"]!;
    string usersBase = _cfg["USERS_BASE_URL"]!;

    if (string.IsNullOrWhiteSpace(tenantsBase) || string.IsNullOrWhiteSpace(usersBase))
        return StatusCode(500, "Upstream base URLs not configured");

    var client = httpClientFactory.CreateClient();

    var tenantResp = await client.GetAsync($"{tenantsBase}/tenants/{dto.TenantId}", ct);
    if (!tenantResp.IsSuccessStatusCode) return StatusCode(401, "No tenant found.");
    var tenantJson = await tenantResp.Content.ReadFromJsonAsync<TenantDto>(cancellationToken: ct);
    if (tenantJson is null || !tenantJson.IsActive) return Unauthorized();

    var userResp = await client.GetAsync($"{usersBase}/users/by-username?tenantId={tenantJson.TenantId}&username={dto.Username}", ct);
    if (!userResp.IsSuccessStatusCode) return Unauthorized();
    var user = await userResp.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct);
    if (user is null || !user.IsActive) return Unauthorized();
    if (string.IsNullOrWhiteSpace(user.PasswordHash) || !VerifyPassword(dto.Password, user.PasswordHash)) return Unauthorized();

    var oldSessions = await _db.Sessions
        .Where(s => s.UserId == user.UserId && s.TenantId == user.TenantId)
        .ToListAsync(ct);
    if (oldSessions.Any()) _db.Sessions.RemoveRange(oldSessions);

    var (jwt, sessionId) = IssueSessionToken(user.UserId, user.TenantId);
    var tokenHash = HashToken(jwt);
    _db.Sessions.Add(new CMS.AuthService.Domain.Session
    {
        SessionId = sessionId,
        UserId = user.UserId,
        TenantId = user.TenantId,
        TokenHash = tokenHash,
        IssuedAtUtc = DateTime.UtcNow,
        ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15)
    });
    await _db.SaveChangesAsync(ct);

    return Ok(new
    {
        token = jwt,
        sessionId = sessionId,
        user = new
        {
            username = user.Username,
            normalizedUsername = user.NormalizedUsername,
            tenantId = user.TenantId,
            role = user.Role,
            isActive = user.IsActive
        }
    });
}


    [HttpGet("session")]
public async Task<IActionResult> GetSession([FromServices] IHttpClientFactory httpClientFactory, CancellationToken ct)
{
    var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
    if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
        return Unauthorized(new { message = "Missing bearer token" });

    var rawToken = authHeader.Substring("Bearer ".Length).Trim();

    JwtSecurityToken jwt;
    try
    {
        jwt = new JwtSecurityTokenHandler().ReadToken(rawToken) as JwtSecurityToken
              ?? throw new Exception("Invalid token");
    }
    catch
    {
        return Unauthorized(new { message = "Invalid token format" });
    }

    var sid = jwt.Claims.FirstOrDefault(c => c.Type == "sid")?.Value;
    var tenantId = jwt.Claims.FirstOrDefault(c => c.Type == "tenant")?.Value;
    var userId = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

    if (!Guid.TryParse(sid, out var sessionGuid) || tenantId == null || userId == null)
        return Unauthorized(new { message = "Invalid token claims" });

    var session = await _db.Sessions.FirstOrDefaultAsync(s => s.SessionId == sessionGuid, ct);
    if (session is null || session.RevokedAtUtc != null || session.ExpiresAtUtc < DateTime.UtcNow)
        return Unauthorized(new { message = "Session invalid" });

    // Fetch user from the users service
    string usersBase = _cfg["USERS_BASE_URL"]!;
    var client = httpClientFactory.CreateClient();
    var userResp = await client.GetAsync($"{usersBase}/users/{userId}", ct);
    if (!userResp.IsSuccessStatusCode) return Unauthorized(new { message = "User not found" });

    var user = await userResp.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct);
    if (user is null) return Unauthorized(new { message = "User not found" });

    return Ok(new
    {
        token = rawToken,
        sessionId = sid,
        user = new
        {
            username = user.Username,
            normalizedUsername = user.NormalizedUsername,
            tenantId = user.TenantId,
            role = user.Role,
            isActive = user.IsActive
        }
    });
}

    [HttpGet("sessions/{id:guid}")]
    public async Task<IActionResult> ValidateSession(Guid id, [FromQuery] string? token, CancellationToken ct)
    {
        var session = await _db.Sessions.FirstOrDefaultAsync(s => s.SessionId == id, ct);
        if (session is null || session.RevokedAtUtc != null || session.ExpiresAtUtc <= DateTime.UtcNow) return Unauthorized();
        if (!string.IsNullOrEmpty(token))
        {
            var hash = HashToken(token);
            if (!string.Equals(hash, session.TokenHash, StringComparison.Ordinal)) return Unauthorized();
        }
        return Ok(new { session.SessionId, session.UserId, session.TenantId, session.ExpiresAtUtc });
    }

    [HttpDelete("sessions/{id:guid}")]
    public async Task<IActionResult> RevokeSession(Guid id, CancellationToken ct)
    {
        var session = await _db.Sessions.FirstOrDefaultAsync(s => s.SessionId == id, ct);
        if (session is null) return NotFound();
        if (session.RevokedAtUtc == null) session.RevokedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Ok();
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        // stored format: base64(salt):base64(hash)
        var parts = storedHash.Split(':');
        if (parts.Length != 2) return false;
        var salt = Convert.FromBase64String(parts[0]);
        var expected = Convert.FromBase64String(parts[1]);
        using var derive = new Rfc2898DeriveBytes(password, salt, 15_000, HashAlgorithmName.SHA256);
        var actual = derive.GetBytes(expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }

    [HttpPost("logout")]
public async Task<IActionResult> Logout(CancellationToken ct)
{
    var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
    if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
        return Unauthorized();

    var token = authHeader.Substring("Bearer ".Length).Trim();
    var handler = new JwtSecurityTokenHandler();
    JwtSecurityToken jwt;

    try
    {
        jwt = handler.ReadToken(token) as JwtSecurityToken ?? throw new Exception();
    }
    catch
    {
        return Unauthorized();
    }

    string? sid = jwt.Claims.FirstOrDefault(c => c.Type == "sid")?.Value;
    if (!Guid.TryParse(sid, out var sessionId))
        return Unauthorized();

    var session = await _db.Sessions.FirstOrDefaultAsync(s => s.SessionId == sessionId, ct);

    if (session == null) return Ok();

    if (session.RevokedAtUtc == null)
        session.RevokedAtUtc = DateTime.UtcNow;

    await _db.SaveChangesAsync(ct);
    return Ok();
}


    private (string jwt, Guid sessionId) IssueSessionToken(Guid userId, Guid tenantId)
    {
        var key = _cfg["Auth:Jwt:Key"] ?? _cfg["JWT_SIGNING_KEY"] ?? "dev-super-secret-key-change";
        var issuer = _cfg["Auth:Jwt:Issuer"] ?? "cms.auth";
        var audience = _cfg["Auth:Jwt:Audience"] ?? "cms.services";
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var handler = new JwtSecurityTokenHandler();
        var sessionId = Guid.NewGuid();
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Expires = DateTime.UtcNow.AddMinutes(15),
            Subject = new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim("sub", userId.ToString()),
                new System.Security.Claims.Claim("tenant", tenantId.ToString()),
                new System.Security.Claims.Claim("sid", sessionId.ToString())
            }),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };
        var token = handler.CreateToken(descriptor);
        return (handler.WriteToken(token), sessionId);
    }

    private static string HashToken(string raw)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return Convert.ToBase64String(bytes);
    }
}

public record LoginDto(string TenantId, string Username, string Password);
public record LogoutDto(Guid SessionId);
public record TenantDto(Guid TenantId, string Code, string Name, bool IsActive);
public record UserDto(Guid UserId, Guid TenantId, string Username, string NormalizedUsername, int Role, bool IsActive, string PasswordHash);