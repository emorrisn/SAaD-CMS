using CMS.ComplaintService.Domain.Entities;

namespace CMS.ComplaintService.Domain.Events;

public record TicketLogged(Ticket Ticket);
