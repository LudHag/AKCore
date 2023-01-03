export interface UpcomingYears {
  [year: string]: UpcomingYear;
}

export interface UpcomingYear {
  year: number;
  months: UpcomingMonths;
}

export interface UpcomingMonths {
  [month: string]: UpcomingEvent[];
}

export interface UpcomingEvent {
  id: number;
  type: string;
  name: string;
  place: string;
  description: string;
  internalDescription: string;
  fika: string;
  year: number;
  month: number;
  day: string;
  dayDate: string;
  dayInMonth: number;
  halanTime: string;
  thereTime: string;
  startsTime: string;
  stand: string;
  secret: boolean;
  signupState: string;
  coming: number;
  notComing: number;
}
