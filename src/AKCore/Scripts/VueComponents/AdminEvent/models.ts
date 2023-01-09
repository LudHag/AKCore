import { UpcomingEvent } from "../Upcoming/models";

export interface AdminEventModel {
  events: UpcomingEvent[];
  id: number;
  secret: boolean;
  name: string;
  place: string;
  type: string;
  fika: string;
  description: string;
  internalDescription: string;
  day: string;
  halan: string;
  there: string;
  starts: string;
  stand: string;
  old: boolean;
  totalPages: number;
  currentPage: number;
}
