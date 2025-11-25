using CMS.UserService.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.UserService.Domain;

namespace CMS.UserService.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly UserDbContext _db;
    public UsersController(UserDbContext db) => _db = db;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == id, ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] Guid tenantId, [FromQuery] UserRole? role, CancellationToken ct)
    {
        var query = _db.Users.AsQueryable().Where(u => u.TenantId == tenantId);
        if (role.HasValue) query = query.Where(u => u.Role == role);
        var users = await query.Take(200).ToListAsync(ct);
        return Ok(users);
    }

    [HttpGet("by-username")]
    public async Task<IActionResult> GetByUsername([FromQuery] Guid tenantId, [FromQuery] string username, CancellationToken ct)
    {
        if (tenantId == Guid.Empty || string.IsNullOrWhiteSpace(username)) return BadRequest();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.TenantId == tenantId && u.Username == username, ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto, CancellationToken ct)
    {
        var exists = await _db.Users.AnyAsync(u => u.TenantId == dto.TenantId && u.Username == dto.Username, ct);
        if (exists) return Conflict("username exists");
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8) return BadRequest("weak password");
        var user = new User { UserId = Guid.NewGuid(), TenantId = dto.TenantId, Username = dto.Username, Role = dto.Role, PasswordHash = HashPassword(dto.Password) };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return Created($"/users/{user.UserId}", user);
    }

    private static string HashPassword(string password)
    {
        var salt = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16);
        using var derive = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, 15_000, System.Security.Cryptography.HashAlgorithmName.SHA256);
        var hash = derive.GetBytes(32);
        return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
    }
}

public record CreateUserDto(Guid TenantId, string Username, UserRole Role, string Password);