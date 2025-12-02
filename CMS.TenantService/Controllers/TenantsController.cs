using CMS.TenantService.Domain;
using CMS.TenantService.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.TenantService.Controllers;

[ApiController]
[Route("tenants")]
public class TenantsController : ControllerBase
{
    private readonly TenantDbContext _db;
    public TenantsController(TenantDbContext db) => _db = db;

    [HttpGet("list")]
    public async Task<IActionResult> GetList(CancellationToken ct)
    {
        return Ok(await _db.Tenants.Where(t => t.IsActive).Select(t => new { t.TenantId, t.Name }).ToListAsync(ct));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByCode(Guid id, CancellationToken ct)
    {
        Tenant? tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.TenantId == id, ct);
        return tenant is null ? NotFound() : Ok(tenant);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTenantDto dto, CancellationToken ct)
    {
        if (await _db.Tenants.AnyAsync(t => t.Code == dto.Code, ct)) return Conflict("tenant code exists");
        var tenant = new Domain.Tenant { TenantId = Guid.NewGuid(), Code = dto.Code, Name = dto.Name, Industry = dto.Industry };
        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync(ct);
        return Created($"/tenants/{tenant.Code}", tenant);
    }

    [HttpPost("{code}/activate")]
    [InternalOnly]
    public async Task<IActionResult> Activate(string code, CancellationToken ct)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Code == code, ct);
        if (tenant is null) return NotFound();
        tenant.IsActive = true;
        await _db.SaveChangesAsync(ct);
        return Ok();
    }

    [HttpPost("{code}/deactivate")]
    [InternalOnly]
    public async Task<IActionResult> Deactivate(string code, CancellationToken ct)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Code == code, ct);
        if (tenant is null) return NotFound();
        tenant.IsActive = false;
        await _db.SaveChangesAsync(ct);
        return Ok();
    }
}

public record CreateTenantDto(string Code, string Name, string? Industry);

// Attribute & middleware helper could be elsewhere; placed inline for brevity.
public sealed class InternalOnlyAttribute : Attribute {}