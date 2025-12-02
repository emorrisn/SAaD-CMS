using CMS.ComplaintService.Application.Commands;
using CMS.ComplaintService.Application.Queries;
using CMS.ComplaintService.Domain.Entities;
using CMS.ComplaintService.Domain.Enums;
using CMS.ComplaintService.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.ComplaintService.Controllers;

[ApiController]
[Route("complaints")]
public class ComplaintsController(
    Application.Handlers.LogComplaintHandler logComplaintHandler,
    IComplaintQueryService queryService,
    ComplaintDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<object>> LogComplaint([FromBody] LogComplaintCommand command, CancellationToken ct)
    {
        var id = await logComplaintHandler.HandleAsync(command, ct);
        return CreatedAtAction(nameof(GetById), new { id, tenantId = command.TenantID }, new { complaintId = id });
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Complaint>>> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
    {
        // Debug: log all headers
        var logger = HttpContext.RequestServices.GetRequiredService<ILogger<ComplaintsController>>();
        logger.LogInformation("Received headers: {Headers}", string.Join(", ", HttpContext.Request.Headers.Select(h => $"{h.Key}={h.Value}")));
        
        var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(tenantId)) return BadRequest("Tenant is required");
        var items = await queryService.GetAllAsync(tenantId, skip, take, ct);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Complaint>> GetById(Guid id, CancellationToken ct)
    {
        var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(tenantId)) return BadRequest("Tenant is required");
        var c = await queryService.GetByIdAsync(tenantId, id, ct);
        return c is null ? NotFound() : Ok(c);
    }

    public record AssignRequest(string TenantID, Guid AgentID);
    public record ResolveRequest(string TenantID, string? Notes);

    [HttpPatch("{id}/assign")]
    [InternalOnly]
    public async Task<ActionResult> Assign(Guid id, [FromBody] AssignRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.TenantID)) return BadRequest("TenantID is required");

        var complaint = await db.Complaints.FirstOrDefaultAsync(c => c.TenantID == req.TenantID && c.ComplaintID == id, ct);
        if (complaint is null) return NotFound();

        // User existence validation now external; assume provided AgentID already validated upstream.
        if (req.AgentID == Guid.Empty) return BadRequest("AgentID invalid");

        complaint.AgentID = req.AgentID;
        complaint.Status = ComplaintStatus.Assigned;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPatch("{id}/resolve")]
    [InternalOnly]
    public async Task<ActionResult> Resolve(Guid id, [FromBody] ResolveRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.TenantID)) return BadRequest("TenantID is required");

        var complaint = await db.Complaints.FirstOrDefaultAsync(c => c.TenantID == req.TenantID && c.ComplaintID == id, ct);
        if (complaint is null) return NotFound();

        complaint.Status = ComplaintStatus.Resolved;
        complaint.ResolvedAt = DateTime.UtcNow;
        // Optional: record notes in audit
        if (!string.IsNullOrWhiteSpace(req.Notes))
        {
            db.ComplaintAuditHistories.Add(new ComplaintAuditHistory
            {
                AuditID = Guid.NewGuid(),
                ComplaintID = complaint.ComplaintID,
                UserID = complaint.AgentID ?? Guid.Empty,
                Action = $"Resolved: {req.Notes}",
                Timestamp = DateTime.UtcNow
            });
        }
        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}

public sealed class InternalOnlyAttribute : Attribute {}
