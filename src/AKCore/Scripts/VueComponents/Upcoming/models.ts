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
  descriptionEng: string;
  internalDescription: string;
  internalDescriptionEng: string;
  fika: string;
  year: number;
  month: number;
  day: string;
  dayDate: string | Date;
  dayInMonth: number;
  halanTime: string;
  thereTime: string;
  startsTime: string;
  playDuration: string;
  stand: string;
  secret: boolean;
  signupState: string;
  coming: number;
  notComing: number;
  allowsSignUps: boolean;
}

export interface UpcomingEventInfo {
  where: UpcomingWhere;
  car: boolean;
  instrument: boolean;
  comment: string;
  isNintendo: boolean;
  members: AvailableMember[];
  event: UpcomingEvent;
  signups: UpcomingSignup[];
}

export interface AvailableMember {
  id: number;
  fullName: string;
}

export interface UpcomingSignup {
  id: number;
  person: string;
  personId: string;
  personName: string;
  where: UpcomingWhere;
  car: boolean;
  instrument: boolean;
  instrumentName: string;
  otherInstruments: string;
  comment: string;
  signupTime: string;
}

export type UpcomingWhere = "Hålan" | "Direkt" | "Kan inte komma" | null;
