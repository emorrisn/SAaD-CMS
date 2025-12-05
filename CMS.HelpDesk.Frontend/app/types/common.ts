export interface DateRange {
  start: Date;
  end: Date;
}

export interface CustomerRecord {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phone?: string;
}

export interface UserRecord {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role?: string;
  avatarUrl?: string;
}

export interface StatPoint {
  label: string;
  value: number;
}

export interface StatisticsSummary {
  open: number;
  pending: number;
  resolved: number;
  closed: number;
  avgResponseSeconds: number;
  resolutionRate: number;
  [key: string]: number;
}

export interface ReportEntry {
  id: string;
  name: string;
  createdAt: string;
  payload?: unknown;
  params?: string;
}

export interface AccountProfile {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  avatarUrl?: string;
  language?: string;
  timezone?: string;
}

export type AccountPreferences = Record<string, unknown>;

export interface InboxItem {
  id: string;
  subject: string;
  from: string;
  receivedAt: string;
  ticketId?: string;
  unread: boolean;
}

export type ThemeChoice = "light" | "dark" | "system";

export interface SystemSettings {
  categories: string[];
  priorities: string[];
  statuses: string[];
  sources: string[];
  theme: ThemeChoice;
  isNavbarCollapsed: boolean;
  isNavbarVisible: boolean;
}
