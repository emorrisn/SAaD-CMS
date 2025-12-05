using CMS.ComplaintService.Application.Commands;
using CMS.ComplaintService.Application.Handlers;
using CMS.ComplaintService.Application.Queries;
using CMS.ComplaintService.Domain.Entities;
using CMS.ComplaintService.Domain.Enums;
using CMS.ComplaintService.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.ComplaintService.Controllers;

[ApiController]
[Route("tickets")]
public class TicketsController(
    CreateTicketHandler createTicketHandler,
    ITicketQueryService queryService,
    TicketDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateTicketCommand command, CancellationToken ct)
    {
        var id = await createTicketHandler.HandleAsync(command, ct);
        return CreatedAtAction(nameof(GetById), new { id, tenantId = command.TenantId }, new { ticketId = id });
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
    {
        var logger = HttpContext.RequestServices.GetRequiredService<ILogger<TicketsController>>();
        logger.LogInformation("Received headers: {Headers}", string.Join(", ", HttpContext.Request.Headers.Select(h => $"{h.Key}={h.Value}")));

        var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(tenantId)) return BadRequest("Tenant is required");
        var items = await queryService.GetAllAsync(tenantId, skip, take, ct);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetById(Guid id, CancellationToken ct)
    {
        var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(tenantId)) return BadRequest("Tenant is required");
        var ticket = await queryService.GetByIdAsync(tenantId, id, ct);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    public record AssignRequest(string TenantId, Guid AssignedId);
    public record ResolveRequest(string TenantId, string? ResolutionNotes, bool IsEscalated);

    [HttpPatch("{id}/assign")]
    [InternalOnly]
    public async Task<ActionResult> Assign(Guid id, [FromBody] AssignRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.TenantId)) return BadRequest("TenantId is required");
        if (req.AssignedId == Guid.Empty) return BadRequest("AssignedId invalid");

        var ticket = await db.Tickets.FirstOrDefaultAsync(t => t.TenantId == req.TenantId && t.TicketId == id, ct);
        if (ticket is null) return NotFound();

        ticket.AssignedId = req.AssignedId;
        ticket.Status = TicketStatus.Assigned;
        ticket.UpdatedAt = DateTime.UtcNow;

        db.TicketLogs.Add(new TicketLog
        {
            TicketLogId = Guid.NewGuid(),
            TicketId = ticket.TicketId,
            PerformedById = req.AssignedId,
            Action = "Assigned",
            Notes = null,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPatch("{id}/resolve")]
    [InternalOnly]
    public async Task<ActionResult> Resolve(Guid id, [FromBody] ResolveRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.TenantId)) return BadRequest("TenantId is required");

        var ticket = await db.Tickets.FirstOrDefaultAsync(t => t.TenantId == req.TenantId && t.TicketId == id, ct);
        if (ticket is null) return NotFound();

        ticket.Status = TicketStatus.Resolved;
        ticket.ResolutionNotes = req.ResolutionNotes;
        ticket.IsEscalated = req.IsEscalated;
        ticket.ResolvedAt = DateTime.UtcNow;
        ticket.UpdatedAt = DateTime.UtcNow;

        db.TicketLogs.Add(new TicketLog
        {
            TicketLogId = Guid.NewGuid(),
            TicketId = ticket.TicketId,
            PerformedById = ticket.AssignedId,
            Action = "Resolved",
            Notes = req.ResolutionNotes,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}

public sealed class InternalOnlyAttribute : Attribute {}
