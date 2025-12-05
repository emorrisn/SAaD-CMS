import type { DateRange } from "./common";

export interface HomeStat {
  title: string;
  icon: string;
  value: number | string;
  variation: number;
  link?: string;
  formatter?: (value: number | string) => string;
}

export interface HomeRange extends DateRange {}
