export type ExportMode = "pdf" | "csv" | "schedule";

export interface ArchiveRow {
  date: string;
  name: string;
  params: string;
  actions: string;
}
