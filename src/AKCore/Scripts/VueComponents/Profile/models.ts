export interface ProfileData {
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  phone: string;
  instrument: string;
  otherInstruments: string[];
  medal: string | null;
  givenMedal: string | null;
  roles: string[];
  posts: string[];
}

export interface ProfileStatisticsModel {
  totalGigs: number;
  halan: number;
  direct: number;
  cantCome: number;
  car: number;
  instrument: number;
  comment: number;
}
