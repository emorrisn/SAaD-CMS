export type TicketPriority = "low" | "medium" | "high" | "urgent" | "unknown";

export type TicketStatus = "open" | "pending" | "resolved" | "closed";

export interface Ticket {
  id: number;
  subject: string;
  requester: {
    firstName: string;
    lastName: string;
    email: string;
    avatar?: { src?: string; alt?: string };
  };
  priority: TicketPriority;
  status: TicketStatus;
  createdAt: string;
  updatedAt: string;
  location?: string;
}

export interface TicketSummary {
  id: string;
  subject: string;
  customer: { firstName: string; lastName: string };
  status: TicketStatus;
  priority: TicketPriority;
  createdAt: string;
}
